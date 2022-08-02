#!/bin/bash
docker run --rm -w /acoustics/ -v "$PWD":/acoustics/working/ mcr.microsoft.com/acoustics/baketools:3.0.Linux ./tools/Triton.LocalProcessor --configfile Acoustics_Base_config.xml --workingdir working
rm *.enc
