import numpy as np
import cv2
import sys

# You should replace these 3 lines with the output in calibration step
DIM=(1920, 1080)
K=np.array([[1435.8734039382937, 0.0, 883.6370522315407], [0.0, 1426.404376643103, 542.7057195251705], [0.0, 0.0, 1.0]])
D=np.array([[-0.16929718287443193], [2.862752075154555], [-22.961417453039385], [55.758246113631145]])
def undistort(img_path):
    img = cv2.imread(img_path)
    h,w = img.shape[:2]
    map1, map2 = cv2.fisheye.initUndistortRectifyMap(K, D, np.eye(3), K, DIM, cv2.CV_16SC2)
    undistorted_img = cv2.remap(img, map1, map2, interpolation=cv2.INTER_LINEAR, borderMode=cv2.BORDER_CONSTANT)
    # cv2.imshow("undistorted", undistorted_img)
    # cv2.waitKey(0)
    # cv2.destroyAllWindows()
    return undistorted_img
if __name__ == '__main__':
    for p in sys.argv[1:]:
        undistort(p)