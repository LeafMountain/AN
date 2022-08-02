docker run --rm -w /acoustics/ -v "%CD%":/acoustics/working/ mcr.microsoft.com/acoustics/baketools:3.0.Linux ./tools/Triton.LocalProcessor --configfile Acoustics_Base_config.xml --workingdir working
del *.enc
