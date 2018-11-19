#!/usr/bin/env python3
from pathlib import Path
from setuptools import setup
from setuptools.config import read_configuration

cfg = read_configuration(Path(__file__).parent / 'setup.cfg')
#print(cfg)
cfg["options"].update(cfg["metadata"])
cfg=cfg["options"]
setup(use_scm_version = True, **cfg)
