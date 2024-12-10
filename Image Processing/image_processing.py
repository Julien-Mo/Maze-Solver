import cv2 as cv
import numpy as np
import json
import os
from typing import List, Tuple, Optional

MAX_VALUE = 255
BGR_LENGTH = 3
GRID_HEIGHT = 63
GRID_WIDTH = 65
IMAGE_HEIGHT = 480
IMAGE_WIDTH = 640
X1, Y1 = 130, 20 # Top Left Corner
X2, Y2 = IMAGE_WIDTH - 120, 40 # Top Right Corner
X3, Y3 = IMAGE_WIDTH - 130, IMAGE_HEIGHT - 60 # Bottom Right Corner
X4, Y4 = 110, IMAGE_HEIGHT - 70 # Bottom Left Corner
KERNEL = (7, 7)
THRESH_BLOCK = 11
THRESH_C = 9
THRESH_LOW = 50
THRESH_HIGH = 150
ITERATIONS = 5
APPROXIMATION_TOLERANCE = 0.02
COLOR_DIFFERENCE_TOLERANCE = 1.2
SKEW_TOLERANCE = 0.95
RECT_LENGTH = 4
RECT_ROW = 4
RECT_COL = 2


class Maze:
    def __init__(self, image_path: str | os.PathLike) -> None:
        self.image = cv.imread(image_path)
        self.original = self.image.copy()
        self.start = None
        self.end = None
        self._kernel = np.ones(KERNEL, np.uint8)

    def run(self) -> None:
        """Applies all the required transformations to the maze."""
        self.crop_image()
        self.fix_skew()
        self.get_binary()
        self.get_markers()
        self.get_output()

    def show(self, name: str="Maze") -> None:
        """Displays the current image of the maze."""
        cv.imshow(name, self.image)
        cv.waitKey(0)
        cv.destroyAllWindows()

    def crop_image(self) -> None:
        """Manually crops the camera input to the playable area"""
        pts = np.array([[X1, Y1], [X2, Y2], [X3, Y3], [X4, Y4]], dtype=np.float32)
        dst_pts = np.array([[0, 0], [IMAGE_WIDTH - 1, 0], [IMAGE_WIDTH - 1, IMAGE_HEIGHT - 1], [0, IMAGE_HEIGHT - 1]], dtype=np.float32)
        matrix = cv.getPerspectiveTransform(pts, dst_pts)
        self.image = cv.warpPerspective(self.image, matrix, (IMAGE_WIDTH, IMAGE_HEIGHT))
        self.original = self.image.copy()

    def fix_skew(self, tolerance: float=SKEW_TOLERANCE) -> None:
        """Corrects the skew in the image by finding the largest rectangular contour and applying a perspective transform."""
        blurred = cv.GaussianBlur(self.image, KERNEL, cv.BORDER_DEFAULT)
        edges = cv.Canny(blurred, THRESH_LOW, THRESH_HIGH)
        edges = cv.dilate(edges, self._kernel, iterations=ITERATIONS)

        # Find largest rectangle to crop image
        contours, _ = cv.findContours(edges, cv.RETR_LIST, cv.CHAIN_APPROX_SIMPLE)
        contours = sorted(contours, key=cv.contourArea, reverse=True)
        rect_points = self._find_largest_rectangle_contour(contours)

        # Calculate adjusted rectangle points
        center = np.mean(rect_points, axis=0)
        rect_points = (rect_points - center) * tolerance + center

        # Apply perspective transformation
        rect = self._order_rectangle_points(rect_points)
        width, height = int(np.linalg.norm(rect[0] - rect[1])), int(np.linalg.norm(rect[1] - rect[2]))
        dst = np.array([[0, 0], [width - 1, 0], [width - 1, height - 1], [0, height - 1]], dtype="float32")
        M = cv.getPerspectiveTransform(rect, dst)
        self.image = cv.warpPerspective(self.image, M, (width, height))
        self.original = self.image.copy()

    def get_binary(self) -> None:
        """Converts the image to binary and dilates the edges."""
        self.image = cv.cvtColor(self.image, cv.COLOR_BGR2GRAY)
        self.image = cv.GaussianBlur(self.image, KERNEL, cv.BORDER_DEFAULT)
        self.image = cv.adaptiveThreshold(self.image, MAX_VALUE, cv.ADAPTIVE_THRESH_MEAN_C, cv.THRESH_BINARY_INV, THRESH_BLOCK, THRESH_C)

    def get_markers(self, tolerance: float=COLOR_DIFFERENCE_TOLERANCE) -> None:
        """Finds the start and end markers in the maze."""
        b, g, r = cv.split(self.original)
        self.start = self._process_marker((r > tolerance * g) & (r > tolerance * b))
        self.end = self._process_marker((g > tolerance * r) & (g > tolerance * b))
        self.image = cv.dilate(self.image, self._kernel, iterations=ITERATIONS)

    def get_output(self, width: int=GRID_WIDTH, height: int=GRID_HEIGHT) -> None:
        """Resizes the processed maze and saves its data as a JSON file."""
        self.image = cv.resize(self.image, (width, height), interpolation=cv.INTER_AREA)
        self.start = (int(self.start[0] * width), int(self.start[1] * height))
        self.end = (int(self.end[0] * width), int(self.end[1] * height))
        self.maze = (self.image != 0).astype(int).tolist()
        output_data = {
            "maze": self.maze,
            "start": self.start,
            "end": self.end,
        }
        with open(f"{os.path.dirname(os.path.realpath(__file__))}/maze_data.json", "w") as f:
            json.dump(output_data, f)

    def _find_largest_rectangle_contour(self, contours: List[np.ndarray], tolerance: float=APPROXIMATION_TOLERANCE) -> np.ndarray:
        """Finds the largest contour with four corners (assumed to be a rectangle)."""
        for contour in contours:
            approx = cv.approxPolyDP(contour, tolerance * cv.arcLength(contour, True), True)
            if len(approx) == RECT_LENGTH:
                return approx.reshape(RECT_ROW, RECT_COL)
        raise ValueError("No rectangular contour found in the image.")

    def _order_rectangle_points(self, points: np.ndarray) -> np.ndarray:
        """Orders points in clockwise order: top-left, top-right, bottom-right, bottom-left."""
        rect = np.zeros((RECT_ROW, RECT_COL), dtype="float32")
        s = points.sum(axis=1)
        rect[0] = points[np.argmin(s)]
        rect[2] = points[np.argmax(s)]
        diff = np.diff(points, axis=1)
        rect[1] = points[np.argmin(diff)]
        rect[3] = points[np.argmax(diff)]
        return rect

    def _process_marker(self, color_condition: np.ndarray) -> Optional[Tuple[float, float]]:
        """Process a single marker based on its color condition."""
        mask = color_condition.astype(np.uint8) * MAX_VALUE
        contours, _ = cv.findContours(mask, cv.RETR_EXTERNAL, cv.CHAIN_APPROX_SIMPLE)
        if not contours:
            return None
        largest_contour = max(contours, key=cv.contourArea)
        contour_mask = np.zeros_like(mask)
        cv.drawContours(contour_mask, [largest_contour], -1, MAX_VALUE, thickness=cv.FILLED)
        dilated_mask = cv.dilate(contour_mask, self._kernel, iterations=ITERATIONS)
        self.image[dilated_mask == MAX_VALUE] = 0
        return self._calculate_center(largest_contour)

    def _calculate_center(self, contour: np.ndarray) -> Optional[Tuple[float, float]]:
        """Calculate the center of a contour."""
        M = cv.moments(contour)
        if M["m00"] != 0:
            return (M["m10"] / M["m00"] / len(self.image[0]), 
                    M["m01"] / M["m00"] / len(self.image))
        return None


if __name__ == "__main__":
    maze = Maze(f"{os.path.dirname(os.path.realpath(__file__))}/../Maze Solver/bin/Debug/captured_image.png")
    maze.run()
    cv.imwrite(f"{os.path.dirname(os.path.realpath(__file__))}/processed_image.png", maze.original)