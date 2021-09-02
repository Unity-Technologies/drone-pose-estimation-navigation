import logging
import asyncio
import grpc
import torch

import PostEstimation_pb2
import PostEstimation_pb2_grpc

from concurrent import futures

from train.pose_estimation.model import PoseEstimationNetwork
from PostEstimation_pb2_grpc import PostEstimationServiceServicer

def exec():
    s = PostEstimationServiceServicer()
    s.GetPoseEstimation()