# Refit.Insane.PowerPack
Refit Insane PowerPack is a Refit library extensions which provides attribute based cache and auto-retry features.

# Installation Guide
Install-Package Refit.Insane.PowerPack

# Documentation
1. Check Sample app which is available in this repository.
2. Read presentation I have created for Cracow #Xamarines - Xamarines.com
https://github.com/thefex/Refit.Insane.PowerPack/blob/master/refit_presentation.pdf

3. In order to use ApiDefinitionAttribute you need to either:
* Attach attributes to each refit API interface
* Attach attribute to only specific API interfaces but also set BaseApiConfiguration.ApiUri which will be used as base uri for all interfaces without ApiDefinition attribute

# Acknowledgment
This library has been created thanks to help and support from InsaneLab.com (http://www.insanelab.com). Thanks for giving me time to work on this project - you guys rock :-)

Thanks to Artur Malendowicz (https://github.com/Immons) for implementing multiple API support (ApiDefinition)

Thanks to Jakub Kaprzyk (https://github.com/qbus00) for implementing ApiDefinition Timeout support.
