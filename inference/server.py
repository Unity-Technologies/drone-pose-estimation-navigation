import logging
import asyncio
import grpc

import PostEstimation_pb2
import PostEstimation_pb2_grpc

from concurrent import futures


class Greeter(PostEstimation_pb2_grpc.ColorGeneratorServicer):

    def GetRandomColor(
            self, request: PostEstimation_pb2.CurrentColor,context) -> PostEstimation_pb2.NewColor:
        return PostEstimation_pb2.NewColor(color='Hello, %s!' % request.color)


def serve() -> None:
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    PostEstimation_pb2_grpc.add_ColorGeneratorServicer_to_server(Greeter(), server)
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
