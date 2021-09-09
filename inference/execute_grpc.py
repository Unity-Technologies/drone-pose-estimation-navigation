from PostEstimation_pb2_grpc import PostEstimationServiceServicer

def exec():
    s = PostEstimationServiceServicer()
    s.GetPoseEstimation()