import logging
import asyncio
import grpc

import PostEstimation_pb2
import PostEstimation_pb2_grpc

from concurrent import futures


class PoseEstimation(PostEstimation_pb2_grpc.PostEstimationServiceServicer):

    def GetPoseEstimation(
            self, request: PostEstimation_pb2.ImageInfo,context) -> PostEstimation_pb2.PoseEstimationResponse:
        return PostEstimation_pb2.PoseEstimationResponse(droneQuaternion=PostEstimation_pb2.Quaternion(x=1,y=2,z=3,w=4), targetQuaternion= PostEstimation_pb2.Quaternion(x=1,y=2,z=3,w=4), droneTransformPosition=PostEstimation_pb2.TransformPosition(x=0,y=0,z=0), targetTransformPosition=PostEstimation_pb2.TransformPosition(x=0,y=0,z=0))
    


def serve() -> None:
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    PostEstimation_pb2_grpc.add_PostEstimationServiceServicer_to_server(PoseEstimation(), server)
    listen_addr = '[::]:50051'
    server.add_insecure_port(listen_addr)
    logging.info("Starting server on %s", listen_addr)
    server.start()
    try:
        server.wait_for_termination()
    except KeyboardInterrupt:
        # Shuts down the server with 0 seconds of grace period. During the
        # grace period, the server won't accept new connections and allow
        # existing RPCs to continue within the grace period.
        server.stop(0)


if __name__ == '__main__':
    logging.basicConfig(level=logging.INFO)
    serve()
