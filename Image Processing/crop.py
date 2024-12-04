import cv2
import numpy as np
import os

def crop_to_defined_rectangle(image_path):
    # Load the image
    image = cv2.imread(image_path)
    if image is None:
        raise ValueError("Image not found or unable to load.")

    # Display the original image
    # cv2.imshow('Original Image', image)
    # cv2.waitKey(0)

    # Define the desired rectangle size
    width, height = 640, 480
    # Define the cropping coordinates (x1, y1, x2, y2, x3, y3, x4, y4)
    # These constants need to be updated based on the specific image
    x1, y1 = 90, 30  # Top-left corner
    x2, y2 = width-90, 30  # Top-right corner
    x3, y3 = width-90, height-40  # Bottom-right corner
    x4, y4 = 90, height-40  # Bottom-left corner

    # Create an array of the points
    pts = np.array([[x1, y1], [x2, y2], [x3, y3], [x4, y4]], dtype=np.float32)

    dst_pts = np.array([
        [0, 0],
        [width - 1, 0],
        [width - 1, height - 1],
        [0, height - 1]
    ], dtype=np.float32)

    # Compute the perspective transform matrix
    matrix = cv2.getPerspectiveTransform(pts, dst_pts)

    # Warp the image to straighten it
    warped = cv2.warpPerspective(image, matrix, (width, height))

    # Display the cropped image
    # cv2.imshow('Cropped Image', warped)
    # cv2.waitKey(0)

    return warped

# Example usage
# Provide the path to your image
image_path = f"{os.path.dirname(os.path.realpath(__file__))}/../Image Capture/bin/Debug/captured_image.png"

try:
    cropped_image = crop_to_defined_rectangle(image_path)
    
    # Display the cropped image
    # cv2.imshow('Cropped Image', cropped_image)
    # cv2.waitKey(0)
    # cv2.destroyAllWindows()
    
    # Optionally, save the cropped image
    cv2.imwrite('cropped_image.jpg', cropped_image)
except ValueError as e:
    print(f"Error: {e}")