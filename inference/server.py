import logging
import asyncio
import grpc

import PostEstimation_pb2
import PostEstimation_pb2_grpc

from concurrent import futures

from pose_estimation import run_model_main


class PoseEstimation(PostEstimation_pb2_grpc.PostEstimationServiceServicer):

    def GetPoseEstimation(
            self, request: PostEstimation_pb2.ImageInfo,context) -> PostEstimation_pb2.PoseEstimationResponse:
        model_choice = "drone_target_pose_estimation_ep50.tar"

        logging.info("Starting pose estimation...")
        output_translation_drone, output_translation_cube = run_model_main(request.image, 640, 480, model_choice)
        print(output_translation_drone,  output_translation_cube)

        output_translation_drone = output_translation_drone[0]
        output_translation_cube = output_translation_cube[0]

        return PostEstimation_pb2.PoseEstimationResponse(
            droneTransformPosition=PostEstimation_pb2.TransformPosition(
                x=output_translation_drone[0],
                y=output_translation_drone[1]),
                #z=output_translation_drone[2]),
            targetTransformPosition=PostEstimation_pb2.TransformPosition(
                x=output_translation_cube[0],
                y=output_translation_cube[1]))
                #z=output_translation_cube[2]))
        

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
