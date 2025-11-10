# built-in dependencies
import traceback
from typing import Optional, Union
import os
from flask import current_app

# 3rd party dependencies
import numpy as np

# project dependencies
from deepface import DeepFace
from deepface.commons.logger import Logger

logger = Logger()


# pylint: disable=broad-except


def verify(
    img_path: Union[str, np.ndarray],
    model_name: str,
    detector_backend: str,
    distance_metric: str,
    enforce_detection: bool,
    align: bool,
    anti_spoofing: bool,
):
    try:
        database_dir = os.path.join(current_app.root_path, "database")

        obj = DeepFace.find(
            img_path=img_path,
            db_path=database_dir,
            model_name=model_name,
            detector_backend=detector_backend,
            distance_metric=distance_metric,
            align=align,
            enforce_detection=enforce_detection,
            anti_spoofing=anti_spoofing,
        )
        return obj
    except Exception as err:
        tb_str = traceback.format_exc()
        logger.error(str(err))
        logger.error(tb_str)
        raise
