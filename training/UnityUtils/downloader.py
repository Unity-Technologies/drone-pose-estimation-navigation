import os
import sys
import tempfile
import requests
from tqdm import tqdm
import zipfile

def download_from_url(url, dst):
    """
    @param: url to download file
    @param: dst place to put the file
    """
    file_size = int(requests.head(url).headers["Content-Length"])
    if os.path.exists(dst):
        first_byte = os.path.getsize(dst)
    else:
        first_byte = 0
    if first_byte >= file_size:
        return file_size
    
    header = {"Range": "bytes=%s-%s" % (first_byte, file_size)}
    pbar = tqdm(total=file_size, initial=first_byte,unit='B', unit_scale=True, desc=url.split('/')[-1])
    req = requests.get(url, headers=header, stream=True)
    with(open(dst, 'ab')) as f:
        for chunk in req.iter_content(chunk_size=1024):
            if chunk:
                f.write(chunk)
            pbar.update(1024)
        pbar.close()
    return file_size

def download_and_unzip(url, dst_folder):
    defult_tmp_dir = tempfile._get_default_tempdir()
    temp_filename = os.path.join(defult_tmp_dir, next(tempfile._get_candidate_names()))
    print('using filename here ==>', temp_filename)
    download_from_url(url, temp_filename)
    print("unzipping...")
    with zipfile.ZipFile(temp_filename, 'r') as zip_ref:
        zip_ref.extractall(dst_folder)
    os.remove(temp_filename)
    
