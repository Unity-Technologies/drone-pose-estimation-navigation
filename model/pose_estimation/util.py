import torchvision
from PIL import Image, ImageOps

def convert_image_to_tensor(image_name, width, height):
    image_origin = Image.open(image_name).convert("RGB")
    transform = get_transform(width, height)
    image = transform(image_origin).unsqueeze(0)

def get_transform(width, height):
    """
    Apply a transform on the input image tensor
    Returns:
        https://pytorch.org/docs/stable/torchvision/transforms.html
    """
    transform = torchvision.transforms.Compose(
        [
            torchvision.transforms.Resize(
                (
                    width,
                    height,
                )
            ),
            torchvision.transforms.ToTensor(),
        ]
    )
    return transform