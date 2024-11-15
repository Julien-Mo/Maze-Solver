import cv2 as cv
import numpy as np
from calibration.undistort import undistort
from remove_dominant_color import remove_dominant_color
import json

# Remove fisheye distortion from image
image = undistort("Image Processing/test_maze3.jpg")
original = image.copy()

# Convert to grayscale TODO: Is this necessary?
gray = cv.cvtColor(image, cv.COLOR_BGR2GRAY)

# Apply Gaussian blur
blurred = cv.GaussianBlur(image, (5, 5), 0)

# Perform edge detection
edges = cv.Canny(blurred, 50, 150)

# Make the edges thicker by applying dilation
kernel = np.ones((7, 7), np.uint8)  # You can change the kernel size to control thickness
dilated_edges = cv.dilate(edges, kernel, iterations=5)

# Find contours on the dilated edges
contours, _ = cv.findContours(dilated_edges, cv.RETR_LIST, cv.CHAIN_APPROX_SIMPLE)
contours = sorted(contours, key=cv.contourArea, reverse=True)

# Find the largest rectangular contour
for contour in contours:
    approx = cv.approxPolyDP(contour, 0.02 * cv.arcLength(contour, True), True)
    if len(approx) == 4:
        screen_contour = approx
        break

# Get the four points of the contour
pts = screen_contour.reshape(4, 2)

# Apply slight inward scaling to the points
scale_factor = 0.9  # Adjust this value to crop more or less (less than 1 crops inward)
center = np.mean(pts, axis=0)  # Center of the rectangle

# Scale each point toward the center
scaled_pts = (pts - center) * scale_factor + center

# Order the points for the perspective transform
rect = np.zeros((4, 2), dtype="float32")
s = scaled_pts.sum(axis=1)
rect[0] = scaled_pts[np.argmin(s)]
rect[2] = scaled_pts[np.argmax(s)]
diff = np.diff(scaled_pts, axis=1)
rect[1] = scaled_pts[np.argmin(diff)]
rect[3] = scaled_pts[np.argmax(diff)]


# Determine the width and height of the new image
(width, height) = (int(np.linalg.norm(rect[0] - rect[1])), int(np.linalg.norm(rect[1] - rect[2])))

# Destination points for perspective transform
dst = np.array([
    [0, 0],
    [width - 1, 0],
    [width - 1, height - 1],
    [0, height - 1]
], dtype="float32")

# Apply the perspective transformation
M = cv.getPerspectiveTransform(rect, dst)
# warped = cv.warpPerspective(dilated_edges, M, (width, height))
warped = cv.warpPerspective(blurred, M, (width, height))


warped, end = remove_dominant_color(warped, "b")
warped, start = remove_dominant_color(warped, "r")


# Convert to grayscale (if not already grayscale)
if len(warped.shape) == 3:
    warped = cv.cvtColor(warped, cv.COLOR_BGR2GRAY)
_, warped = cv.threshold(warped, 128, 255, cv.THRESH_BINARY)

# Perform edge detection
# edges = cv.Canny(warped, 50, 150)
edges = cv.bitwise_not(warped)
# Make the edges thicker by applying dilation
kernel = np.ones((7, 7), np.uint8)  # You can change the kernel size to control thickness
dilated_edges = cv.dilate(edges, kernel, iterations=5)

# Resize to desired grid size (e.g., 50x50 for simplicity)
GridSize = 100  # Change this to your preferred grid size
resized_binary = cv.resize(dilated_edges, (GridSize, GridSize), interpolation=cv.INTER_NEAREST)
start = (int(start[0] * GridSize), int(start[1] * GridSize))
end = (int(end[0] * GridSize), int(end[1] * GridSize))

# Show the result
# cv.imshow("Image", resized_binary)
# cv.imshow("Stdart", warped)
cv.waitKey(0)
cv.destroyAllWindows()

# Convert to 0s and 1s
maze_array = (resized_binary != 0).astype(int)  # 0 for white, 1 for black

# Assuming 'maze_array' is your final 2D array (GridSize x GridSize)
maze_array = maze_array.tolist()  # Convert numpy array to list

# Convert maze and coordinates to a dictionary for JSON
output_data = {
    "maze": maze_array,
    "start": start,
    "end": end
}

# Save the array as a JSON file
with open("Image Processing/maze_data.json", "w") as f:
    json.dump(output_data, f)
    print(output_data["end"])
    print(output_data["start"])
