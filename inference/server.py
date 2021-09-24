import logging
import asyncio
import grpc

import PostEstimation_pb2
import PostEstimation_pb2_grpc

from concurrent import futures

from pose_estimation import run_model_main
import os
os.environ["KMP_DUPLICATE_LIB_OK"] = "TRUE"

class PoseEstimation(PostEstimation_pb2_grpc.PostEstimationServiceServicer):

    def GetPoseEstimation(
            self, request: PostEstimation_pb2.ImageInfo,context) -> PostEstimation_pb2.PoseEstimationResponse:
        model_choice = "model_medium.tar"

        logging.info("Starting pose estimation...")
        output_translation_drone, output_translation_target = run_model_main(request.image, 224, 224, model_choice)
        
        logging.info(f"the predicted (x, y) coordinates of the drone are: {output_translation_drone}")
        logging.info(f"the predicted (x, y) coordinates of the drone are: {output_translation_target}")

        output_translation_drone = output_translation_drone[0]
        output_translation_cube = output_translation_target[0]

        return PostEstimation_pb2.PoseEstimationResponse(
            droneTransformPosition=PostEstimation_pb2.TransformPosition(
                x=output_translation_drone[0],
                y=output_translation_drone[1]),
            targetTransformPosition=PostEstimation_pb2.TransformPosition(
                x=output_translation_cube[0],
                y=output_translation_cube[1]))
        

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
