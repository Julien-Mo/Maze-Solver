import cv2
import numpy as np

def remove_dominant_color(image, color_channel, threshold=1.2):
    """
    Replaces the largest region of a specified color dominance with white.
    
    Args:
        image (ndarray): Input image in BGR format.
        color_channel (str): One of 'r', 'g', 'b' to specify the dominant color.
        threshold (float): The threshold factor to identify the dominant color. Default is 1.2.
    
    Returns:
        tuple: Coordinates of the center of the largest dominant color region (x, y).
        ndarray: Image with the largest dominant color region replaced by white.
    """
    
    # Split the image into its R, G, B channels
    b, g, r = cv2.split(image)

    # Create a dictionary to easily access the correct color channel
    channels = {'r': r, 'g': g, 'b': b}
    
    if color_channel not in channels:
        raise ValueError("color_channel must be one of 'r', 'g', or 'b'")

    # Get the selected channel
    target_channel = channels[color_channel]
    
    # Create a mask for areas where the specified channel is dominant
    if color_channel == "b":
        mask = (target_channel > threshold * g) & (target_channel > threshold * r)
    elif color_channel == "g":
        mask = (target_channel > threshold * r) & (target_channel > threshold * b)
    elif color_channel == "r":
        mask = (target_channel > threshold * g) & (target_channel > threshold * b)
    
    # Convert mask to uint8 (0 or 255) for contour detection
    mask = mask.astype(np.uint8) * 255
    
    # Find contours in the mask
    contours, _ = cv2.findContours(mask, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

    # If there are contours, find the largest one
    center = None
    if contours:
        largest_contour = max(contours, key=cv2.contourArea)
        
        # Create a mask for the largest contour
        largest_contour_mask = np.zeros_like(mask)
        cv2.drawContours(largest_contour_mask, [largest_contour], -1, 255, thickness=cv2.FILLED)

        # Dilate the mask to make the white circle bigger
        kernel = np.ones((15, 15), np.uint8)  # Adjust the size for more or less dilation
        dilated_mask = cv2.dilate(largest_contour_mask, kernel, iterations=3)

        # Replace the pixels in the dilated mask with white
        image[dilated_mask == 255] = [255, 255, 255]

        # Calculate the moments of the largest contour to find the center
        M = cv2.moments(largest_contour)
        if M["m00"] != 0:  # Prevent division by zero
            center = (float(M["m10"] / M["m00"] / len(image[0])), float(M["m01"] / M["m00"] / len(image)))
    return image, center

if __name__ == "__main__":
    # Example usage:
    image = cv2.imread("Image Processing/WIN_20241114_15_11_47_Pro.jpg")

    # Specify which color to remove (r, g, or b) and the threshold
    output_image, center  = remove_dominant_color(image, color_channel='b', threshold=1.2)

    if center:
        print(f"Center of the circle: {center}")
    else:
        print("No dominant color region found.")

    # Display the result
    cv2.imshow("Processed Image", output_image)
    cv2.waitKey(0)
    cv2.destroyAllWindows()

    # Optionally, save the result
    cv2.imwrite("output_image.jpg", output_image)
