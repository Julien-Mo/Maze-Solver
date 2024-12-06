import cv2 as cv
import numpy as np
import json
import os

MAX_VALUE = 255
BGR_LENGTH = 3
GRID_HEIGHT = 63
GRID_WIDTH = 65
IMAGE_HEIGHT = 480
IMAGE_WIDTH = 640
X1, Y1 = 100, 10 # Top Left Corner
X2, Y2 = IMAGE_WIDTH - 130, 10 # Top Right Corner
X3, Y3 = IMAGE_WIDTH - 130, IMAGE_HEIGHT - 60 # Bottom Right Corner
X4, Y4 = 100, IMAGE_HEIGHT - 70 # Bottom Left Corner
KERNEL = (7, 7)
BINARY_BLUR_KERNEL = (5, 5)
THRESH_BLOCK = 11
THRESH_C = 9
THRESH_LOW = 50
THRESH_HIGH = 150
MARKER_CLEANUP_ITERATIONS = 13 # 1
MARKER_PROCESSING_ITERATIONS = 6
SKEW_CORRECTION_ITERATIONS = 5
APPROXIMATION_TOLERANCE = 0.02
COLOR_DIFFERENCE_TOLERANCE = 1.2
SKEW_TOLERANCE = 0.95
RECT_LENGTH = 4
RECT_ROW = 4
RECT_COL = 2

class Maze:
    def __init__(self, image_path):
        self.image_path = image_path
        self.image = cv.imread(image_path)
        self.original = self.image.copy()
        self.start = None
        self.end = None
        self._kernel = np.ones(KERNEL, np.uint8)

    def run(self):
        # self.fix_distortion()
        self.manual_crop()
        self.fix_skew()
        self.get_binary()
        self.get_markers()
        # self.show()
        self.get_output()
        # self.show()

    def show(self, name="Maze"):
        """Displays the current image of the maze."""
        cv.imshow(name, self.image)
        cv.waitKey(0)
        cv.destroyAllWindows()

    def fix_distortion(self):
        """Undistorts the image using calibration parameters."""
        # Get DIM, K, D from calibrate.py
        DIM=(640, 480)
        K=np.array([[343.53376818858186, 0.0, 319.8869355537942], [0.0, 338.5426761188926, 238.76602532161644], [0.0, 0.0, 1.0]])
        D=np.array([[-0.11645487487325135], [1.8724757043932152], [-8.228662489874644], [14.848698958677211]])
        map1, map2 = cv.fisheye.initUndistortRectifyMap(K, D, np.eye(3), K, DIM, cv.CV_16SC2)
        self.image = cv.remap(self.image, map1, map2, interpolation=cv.INTER_LINEAR, borderMode=cv.BORDER_CONSTANT)# Adjust the balance (0: retains more original distortion, 1: maximum correction)

    def manual_crop(self):
        pts = np.array([[X1, Y1], [X2, Y2], [X3, Y3], [X4, Y4]], dtype=np.float32)
        dst_pts = np.array([[0, 0], [IMAGE_WIDTH - 1, 0], [IMAGE_WIDTH - 1, IMAGE_HEIGHT - 1], [0, IMAGE_HEIGHT - 1]], dtype=np.float32)
        matrix = cv.getPerspectiveTransform(pts, dst_pts)
        self.image = cv.warpPerspective(self.image, matrix, (IMAGE_WIDTH, IMAGE_HEIGHT))
        self.original = self.image.copy()

    def fix_skew(self, tolerance=SKEW_TOLERANCE):
        """Corrects the skew in the image by finding the largest rectangular contour and applying a perspective transform."""
        # Apply Gaussian blur
        blurred = cv.GaussianBlur(self.image, KERNEL, cv.BORDER_DEFAULT)
        # cv.imshow("Blurred Image", blurred)

        # Detect edges
        edges = cv.Canny(blurred, THRESH_LOW, THRESH_HIGH)
        # cv.imshow("Edges Detected", edges)

        # Dilate edges
        edges = cv.dilate(edges, self._kernel, iterations=SKEW_CORRECTION_ITERATIONS)
        # cv.imshow("Dilated Edges", edges)

        # Find largest rectangle to crop image
        contours, _ = cv.findContours(edges, cv.RETR_LIST, cv.CHAIN_APPROX_SIMPLE)
        contours = sorted(contours, key=cv.contourArea, reverse=True)
        rect_points = self._find_largest_rectangle_contour(contours)

        # Draw contours on the image for visualization
        # temp_image = self.image.copy()
        # cv.drawContours(temp_image, [np.int0(rect_points)], -1, (0, 255, 0), 2)
        # cv.imshow("Largest Rectangle Contour", temp_image)

        # Calculate adjusted rectangle points
        center = np.mean(rect_points, axis=0)
        rect_points = (rect_points - center) * tolerance + center

        # Apply perspective transformation
        rect = self._order_rectangle_points(rect_points)
        width, height = int(np.linalg.norm(rect[0] - rect[1])), int(np.linalg.norm(rect[1] - rect[2]))
        dst = np.array([[0, 0], [width - 1, 0], [width - 1, height - 1], [0, height - 1]], dtype="float32")
        M = cv.getPerspectiveTransform(rect, dst)
        self.image = cv.warpPerspective(self.image, M, (width, height))

        # Show final transformed image
        # cv.imshow("Corrected Skew Image", self.image)

        # Update the original image reference
        self.original = self.image.copy()

    def get_binary(self):
        """Converts the image to binary and dilates the edges."""
        if len(self.image.shape) == BGR_LENGTH:
            self.image = cv.cvtColor(self.image, cv.COLOR_BGR2GRAY)
        self.image = cv.GaussianBlur(self.image, BINARY_BLUR_KERNEL, cv.BORDER_DEFAULT)
        self.image = cv.adaptiveThreshold(self.image, MAX_VALUE, cv.ADAPTIVE_THRESH_GAUSSIAN_C, cv.THRESH_BINARY, THRESH_BLOCK, THRESH_C)
        self.image = cv.bitwise_not(self.image)

    def get_markers(self, tolerance=COLOR_DIFFERENCE_TOLERANCE):
        """Finds the start and end markers in the maze."""
        # Change self._process_marker input based on start/end marker colors
        b, g, r = cv.split(self.original)
        self.start = self._process_marker((r > tolerance * g) & (r > tolerance * b))
        self.end = self._process_marker((g > tolerance * r) & (g > tolerance * b))
        self.image = cv.dilate(self.image, self._kernel, iterations=5)

    def get_output(self, width=GRID_WIDTH, height=GRID_HEIGHT):
        """Resizes the image and saves the maze data to a JSON file."""
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

    def _find_largest_rectangle_contour(self, contours):
        """Finds the largest contour with four corners (assumed to be a rectangle)."""
        for contour in contours:
            approx = cv.approxPolyDP(contour, APPROXIMATION_TOLERANCE * cv.arcLength(contour, True), True)
            if len(approx) == RECT_LENGTH:
                return approx.reshape(RECT_ROW, RECT_COL)
        raise ValueError("No rectangular contour found in the image.")

    def _order_rectangle_points(self, points):
        """Orders points in clockwise order: top-left, top-right, bottom-right, bottom-left."""
        rect = np.zeros((RECT_ROW, RECT_COL), dtype="float32")
        s = points.sum(axis=1)
        rect[0] = points[np.argmin(s)]
        rect[2] = points[np.argmax(s)]
        diff = np.diff(points, axis=1)
        rect[1] = points[np.argmin(diff)]
        rect[3] = points[np.argmax(diff)]
        return rect

    def _process_marker(self, color_condition):
        """Process a single marker based on its color condition."""
        # Create a mask for the color condition
        mask = color_condition.astype(np.uint8) * MAX_VALUE
        # cv.imshow("Initial Mask", mask)
        # cv.waitKey(0)

        # Find contours on the mask
        contours, _ = cv.findContours(mask, cv.RETR_EXTERNAL, cv.CHAIN_APPROX_SIMPLE)
        if not contours:
            return None

        # Identify the largest contour
        largest_contour = max(contours, key=cv.contourArea)
        contour_mask = np.zeros_like(mask)
        cv.drawContours(contour_mask, [largest_contour], -1, MAX_VALUE, thickness=cv.FILLED)
        # cv.imshow("Contour Mask", contour_mask)
        # cv.waitKey(0)

        # Dilate the contour mask
        dilated_mask = cv.dilate(contour_mask, self._kernel, iterations=MARKER_PROCESSING_ITERATIONS)
        # cv.imshow("Dilated Mask", dilated_mask)
        # cv.waitKey(0)

        # Modify the image to remove marker areas
        self.image[dilated_mask == MAX_VALUE] = 0
        # cv.imshow("Updated Image", self.image)
        # cv.waitKey(0)

        # Calculate the center of the marker
        return self._calculate_center(largest_contour)

    def _calculate_center(self, contour):
        """Calculate the center of a contour."""
        M = cv.moments(contour)
        if M["m00"] != 0:
            return (M["m10"] / M["m00"] / len(self.image[0]), 
                    M["m01"] / M["m00"] / len(self.image))
        return None


if __name__ == "__main__":
    # maze = Maze(f"{os.path.dirname(os.path.realpath(__file__))}/test_maze4.jpg")
    # maze = Maze(f"{os.path.dirname(os.path.realpath(__file__))}/../Image Capture/bin/Debug/captured_image.png")
    # maze = Maze(f"{os.path.dirname(os.path.realpath(__file__))}/../Pathfinding Algorithm/bin/Debug/captured_image.png")
    maze = Maze(f"{os.path.dirname(os.path.realpath(__file__))}/../Maze Solver/bin/Debug/captured_image.png")
    maze.run()
    cv.imwrite(f"{os.path.dirname(os.path.realpath(__file__))}/processed_image.jpg", maze.original)
    # maze.show()