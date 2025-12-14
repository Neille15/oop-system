# Installing Python dependencies

This document explains how to install the Python dependencies for the face recognition service using `requirements.txt`.

## Prerequisites
- Python 3.x installed (use `python --version`).
- `pip` available (upgrade with `python -m pip install --upgrade pip`).
- Recommended: use a virtual environment to avoid system-wide package conflicts.

## Using `venv` (Windows - PowerShell)
```powershell
python -m venv venv
.\\venv\\Scripts\\Activate.ps1
python -m pip install --upgrade pip
pip install -r requirements.txt
```

## Using `venv` (Windows - cmd)
```cmd
python -m venv venv
venv\Scripts\activate
python -m pip install --upgrade pip
pip install -r requirements.txt
```

## macOS / Linux
```bash
python3 -m venv venv
source venv/bin/activate
python -m pip install --upgrade pip
pip install -r requirements.txt
```

## Run the service
- Start the development server:
```bash
python api.py -p 5000
```

- For production use with Gunicorn:
```bash
gunicorn "app:create_app()" -b 0.0.0.0:5000
```

## Troubleshooting
- If `pip install -r requirements.txt` fails when installing `tensorflow` or other binary packages on Windows, consider one of the following:
  - Use a matching Python version (older TensorFlow releases may require Python 3.6/3.7).
  - Use `pip` wheels from the project's official sources or use `conda` to install TensorFlow (easier on Windows).
- If you see build errors for packages like `opencv-python`, installing Visual C++ Build Tools (Windows) or appropriate system packages (Linux/macOS) can help.
- If you need GPU support, install the appropriate TensorFlow GPU package and GPU drivers/CUDA compatible with the TensorFlow version.

If you'd like, I can add example `curl` requests for `/verify` and `/addFace` to this document — want me to add them?
