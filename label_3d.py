import numpy as np
import cv2
import math
import os
import glob
import json
from matplotlib.patches import Polygon
from matplotlib.collections import PatchCollection


def quaternion_rotation_matrix(Q):
    """
    Covert a quaternion into a full three-dimensional rotation matrix.

    Input
    :param Q: A 4 element array representing the quaternion (q0,q1,q2,q3)

    Output
    :return: A 3x3 element matrix representing the full 3D rotation matrix.
             This rotation matrix converts a point in the local reference
             frame to a point in the global reference frame.
    """
    # Extract the values from Q
    q0 = Q[0]
    q1 = Q[1]
    q2 = Q[2]
    q3 = Q[3]

    # First row of the rotation matrix
    r00 = 2 * (q0 * q0 + q1 * q1) - 1
    r01 = 2 * (q1 * q2 - q0 * q3)
    r02 = 2 * (q1 * q3 + q0 * q2)

    # Second row of the rotation matrix
    r10 = 2 * (q1 * q2 + q0 * q3)
    r11 = 2 * (q0 * q0 + q2 * q2) - 1
    r12 = 2 * (q2 * q3 - q0 * q1)

    # Third row of the rotation matrix
    r20 = 2 * (q1 * q3 - q0 * q2)
    r21 = 2 * (q2 * q3 + q0 * q1)
    r22 = 2 * (q0 * q0 + q3 * q3) - 1

    # 3x3 rotation matrix
    rot_matrix = np.array([[r00, r01, r02],
                           [r10, r11, r12],
                           [r20, r21, r22]])

    return rot_matrix


def get_capture(file_path, idx):
    f = open(file_path)
    captures = json.load(f)["captures"]
    return captures[idx]


def get_annotation(file_path, index, dims):
    """
    :param file_path: capture_*.json path
    :param index: index of annotation
    :param dims: image dimention
    :return: (3d-points, camera-intrinsic, translation, rotation-in-quaternion)
    """
    (h, w) = dims
    capture = get_capture(file_path, idx=index)
    annotation = capture["annotations"][0]["values"][0]  # drone
    t = np.array(list(annotation["translation"].values()))
    Q = np.array(list(annotation["rotation"].values()))  # Rotation in quaternion
    K = capture["sensor"]["camera_intrinsic"]  # This is in OpenGL
    # Convert K to OpenCV pinhole camera model intrinsics
    K = open_gl_K_to_open_cv(K, (h, w))

    (t_x, t_y, t_z) = t
    a, b, c = annotation["size"].values()

    # Meters to pixel conversion
    scale = 1
    a, b, c = a * scale, b * scale, c * scale
    w, x, y, z = annotation["rotation"].values()
    points = [
        [t_x + a / 2, t_y + b / 2, t_z - c / 2],
        [t_x + a / 2, t_y + b / 2, t_z + c / 2],
        [t_x + a / 2, t_y - b / 2, t_z - c / 2],
        [t_x + a / 2, t_y - b / 2, t_z + c / 2],
        [t_x - a / 2, t_y + b / 2, t_z - c / 2],
        [t_x - a / 2, t_y + b / 2, t_z + c / 2],
        [t_x - a / 2, t_y - b / 2, t_z - c / 2],
        [t_x - a / 2, t_y - b / 2, t_z + c / 2]
    ]

    return np.asarray(points), K, t, Q


def open_gl_K_to_open_cv(K, dims):
    h, w = dims
    K[0][0] = K[0][0] * w * 0.5
    K[1][1] = K[1][1] * h * 0.5
    # For some reason from perception SDK its -1.00061. Converting to 1 for K
    K[2][2] = abs(int(K[2][2]))
    return np.asarray(K)


def get_2d_from_3d(pts3d, camera_intrinsic_m, dims, lib=True):
    """
    :param lib:
    :param pts3d:
    :param camera_intrinsic_m: Camera intrinsics ( in pixel space )
    :return: 2d points
    """
    h, w = dims
    pts2d = []
    for pt3d in pts3d:
        if lib:
            point_2d, _ = cv2.projectPoints(pt3d, np.identity(3), (0, 0, 0), np.asarray(camera_intrinsic_m), (0, 0, 0, 0))
            c_x = point_2d.squeeze()[0]
            c_y = point_2d.squeeze()[1]
        else:
            m = camera_intrinsic_m @ pt3d
            z = m[-1]  # distance of 3d point from 2d plane
            m = (m / z)  # * -1
            c_x, c_y = m[:2]
        c_x = (c_x + w / 2) / w
        c_y = (c_y + h / 2) / h
        pts2d.append((w * c_x, h * (1 - c_y)))
    return pts2d


def draw_point(img, points2d, dims):
    """
    :param img: opencv img
    :return: opencv img
    """
    h, w = dims
    for point in points2d:
        c = (int(point[0]), int(point[1]))
        img = cv2.circle(img, c, 5, (0, 0, 255), -1)
    return img


def get_camera_data(sensor_data, dims):
    # This is OpenGL. Convert to OpenCV Pinhold camera model
    K = sensor_data["camera_intrinsic"]
    K = open_gl_K_to_open_cv(K, dims)  # In pixels pace
    # Sensor translation
    t = np.array(sensor_data["translation"])
    # Sensor Rotation in quaternion
    Q = np.array(sensor_data["rotation"])

    return K, t, Q


def get_box_corners(annotations):
    boxes = []
    for annotation in annotations:
        a, b, c = np.array(list(annotation["size"].values()))
        t = np.array(list(annotation["translation"].values()))
        t_x, t_y, t_z = t
        box_corners = [
            [t_x + a / 2, t_y + b / 2, t_z - c / 2],
            [t_x + a / 2, t_y + b / 2, t_z + c / 2],
            [t_x + a / 2, t_y - b / 2, t_z - c / 2],
            [t_x + a / 2, t_y - b / 2, t_z + c / 2],
            [t_x - a / 2, t_y + b / 2, t_z - c / 2],
            [t_x - a / 2, t_y + b / 2, t_z + c / 2],
            [t_x - a / 2, t_y - b / 2, t_z - c / 2],
            [t_x - a / 2, t_y - b / 2, t_z + c / 2]
        ]
        boxes.append(np.asarray(box_corners))
    return boxes


def draw_bounding_box(img, capture):
    YELLOW = (0, 255, 255)
    h, w = img.shape[:2]
    camera_intrinsic, camera_t, camera_R = get_camera_data(capture["sensor"], (h, w))
    # Shape: (no. of annotations, 8) -> 8 for each bounding rectangle
    boxes = get_box_corners(capture["annotations"][0]["values"])

    for corners in boxes:
        corners_2d = get_2d_from_3d(corners, camera_intrinsic, dims=(h, w))
        p = [(int(c_2d[0]), int(c_2d[1])) for c_2d in corners_2d]
        # Drawining lines
        img = cv2.line(img, p[0], p[1], YELLOW, 1)
        img = cv2.line(img, p[1], p[3], YELLOW, 1)
        img = cv2.line(img, p[3], p[2], YELLOW, 1)
        img = cv2.line(img, p[2], p[0], YELLOW, 1)
        img = cv2.line(img, p[4], p[5], YELLOW, 1)
        img = cv2.line(img, p[5], p[7], YELLOW, 1)
        img = cv2.line(img, p[7], p[6], YELLOW, 1)
        img = cv2.line(img, p[6], p[4], YELLOW, 1)
        img = cv2.line(img, p[0], p[4], YELLOW, 1)
        img = cv2.line(img, p[1], p[5], YELLOW, 1)
        img = cv2.line(img, p[3], p[6], YELLOW, 1)
        img = cv2.line(img, p[2], p[7], YELLOW, 1)
        img = draw_point(img, corners_2d, dims=(h, w))
    return img


def pre_process_captures(data_path):

    capture_index = {}

    for cap_file in glob.glob(f"{data_path}/Dataset*/captures_*.json"):
        f = open(cap_file)
        cap = json.load(f)["captures"]
        for c in cap:
            f_name = os.path.basename(c["filename"])
            capture_index[f_name] = c

    with open("processed_captures.json", "w") as processed_captures:
        json.dump(capture_index, processed_captures)


if __name__ == "__main__":
    data_dir = "data/drone_training_medium"
    # pre_process_captures(data_dir)

    write_dir = "data/drone_training_medium/3d_labeled"
    j = open("processed_captures.json")
    captures = json.load(j)
    print(len(captures))

    images = glob.glob(f"{data_dir}/RGB*/**.png")
    images.reverse()
    mapping = ""
    mapping_file = open("mapping.csv", "w")

    for idx, img_path in enumerate(images):
        img_filename = os.path.basename(img_path)
        capture = captures[img_filename]
        img = cv2.imread(img_path)
        img = draw_bounding_box(img, capture)
        cv2.imwrite(f"{write_dir}/{os.path.basename(img_path)}", img)

    mapping_file.write(mapping)
    mapping_file.close()



