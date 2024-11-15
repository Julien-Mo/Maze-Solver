import cv2 as cv
import numpy as np
import json

MAX_VALUE = 255
BGR_LENGTH = 3
GRID_SIZE = 100
KERNEL = (7, 7)
BINARY_BLUR_KERNEL = (5, 5)
THRESH_BLOCK = 11
THRESH_C = 9
THRESH_LOW = 50
THRESH_HIGH = 150
MARKER_CLEANUP_ITERATIONS = 15
MARKER_PROCESSING_ITERATIONS = 25
SKEW_CORRECTION_ITERATIONS = 5
APPROXIMATION_TOLERANCE = 0.02
COLOR_DIFFERENCE_TOLERANCE = 1.2
SKEW_TOLERANCE = 0.9
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

    def show(self, name="Maze"):
        """Displays the current image of the maze."""
        cv.imshow(name, self.image)
        cv.waitKey(0)
        cv.destroyAllWindows()

    def fix_distortion(self):
        """Undistorts the image using calibration parameters."""
        # Get DIM, K, D from calibrate.py
        DIM = (1920, 1080)
        K = np.array([[1435.8734039382937, 0.0, 883.6370522315407], [0.0, 1426.404376643103, 542.7057195251705], [0.0, 0.0, 1.0]])
        D = np.array([[-0.16929718287443193], [2.862752075154555], [-22.961417453039385], [55.758246113631145]])
        map1, map2 = cv.fisheye.initUndistortRectifyMap(K, D, np.eye(3), K, DIM, cv.CV_16SC2)
        self.image = cv.remap(self.image, map1, map2, interpolation=cv.INTER_LINEAR, borderMode=cv.BORDER_CONSTANT)

    def fix_skew(self, tolerance=SKEW_TOLERANCE):
        """Corrects the skew in the image by finding the largest rectangular contour and applying a perspective transform."""
        blurred = cv.GaussianBlur(self.image, KERNEL, cv.BORDER_DEFAULT)
        edges = cv.Canny(blurred, THRESH_LOW, THRESH_HIGH)
        edges = cv.dilate(edges, self._kernel, iterations=SKEW_CORRECTION_ITERATIONS)

        # Find largest rectangle to crop image
        contours, _ = cv.findContours(edges, cv.RETR_LIST, cv.CHAIN_APPROX_SIMPLE)
        contours = sorted(contours, key=cv.contourArea, reverse=True)
        rect_points = self._find_largest_rectangle_contour(contours)
        center = np.mean(rect_points, axis=0)
        rect_points = (rect_points - center) * tolerance + center

        # Apply perspective transformation
        rect = self._order_rectangle_points(rect_points)        
        width, height = int(np.linalg.norm(rect[0] - rect[1])), int(np.linalg.norm(rect[1] - rect[2]))
        dst = np.array([[0, 0], [width - 1, 0], [width - 1, height - 1], [0, height - 1]], dtype="float32")
        M = cv.getPerspectiveTransform(rect, dst)
        self.image = cv.warpPerspective(self.image, M, (width, height))
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
        self.end = self._process_marker((b > tolerance * g) & (b > tolerance * r))
        self.image = cv.dilate(self.image, self._kernel, iterations=MARKER_CLEANUP_ITERATIONS)

    def get_output(self, grid_size=GRID_SIZE):
        """Resizes the image and saves the maze data to a JSON file."""
        self.image = cv.resize(self.image, (grid_size, grid_size), interpolation=cv.INTER_NEAREST)
        self.start = (int(self.start[0] * grid_size), int(self.start[1] * grid_size))
        self.end = (int(self.end[0] * grid_size), int(self.end[1] * grid_size))
        self.maze = (self.image != 0).astype(int).tolist()
        output_data = {
            "maze": self.maze,
            "start": self.start,
            "end": self.end,
        }
        with open("Image Processing/maze_data.json", "w") as f:
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
        mask = color_condition.astype(np.uint8) * MAX_VALUE
        contours, _ = cv.findContours(mask, cv.RETR_EXTERNAL, cv.CHAIN_APPROX_SIMPLE)
        if not contours:
            return None
        largest_contour = max(contours, key=cv.contourArea)
        contour_mask = np.zeros_like(mask)
        cv.drawContours(contour_mask, [largest_contour], -1, MAX_VALUE, thickness=cv.FILLED)
        dilated_mask = cv.dilate(contour_mask, self._kernel, iterations=MARKER_PROCESSING_ITERATIONS)
        self.image[dilated_mask == MAX_VALUE] = 0
        return self._calculate_center(largest_contour)

    def _calculate_center(self, contour):
        """Calculate the center of a contour."""
        M = cv.moments(contour)
        if M["m00"] != 0:
            return (M["m10"] / M["m00"] / len(self.image[0]), 
                    M["m01"] / M["m00"] / len(self.image))
        return None


if __name__ == "__main__":
    maze = Maze(f"Image Processing/test_maze4.jpg")
    maze.fix_distortion()
    maze.fix_skew()
    maze.get_binary()
    maze.get_markers()
    maze.get_output()
    maze.show()