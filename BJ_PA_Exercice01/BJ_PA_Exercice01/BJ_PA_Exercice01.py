#Author : Jonathan Borel-Jaquet
#Date : 31/08/2020
#Description : Program for reading the header of a bmp file as well as its modification

import struct
import os
filename = "logo_sav.bmp"
file = open(filename,"rb")
newfile = open("invert.bmp","wb")
data = file.read()
file_signature = data[:2].decode("utf-8")
file_size = int.from_bytes(data[2:5],"little")
file_header_size = int.from_bytes(data[14:18],"little")
file_width_pixel = int.from_bytes(data[18:22],"little")
file_height_pixel = int.from_bytes(data[22:26],"little")
file_nbr_bits_per_pixel = int.from_bytes(data[28:30],"little")
file_image_size = int.from_bytes(data[34:38],"little")

print(f"Signature du fichier : {file_signature}")
print(f"Taille totale du fichier en octets : {file_size}")
print(f"Taille en-tête du fichier en octets : {file_header_size}")
print(f"Largeur image : {file_width_pixel}")
print(f"Hauteur image : {file_height_pixel}")
print(f"Nombre de bits par pixel : {file_nbr_bits_per_pixel}")
print(f"Taille en octets des données de l’image : {file_image_size}")

dst_data_lst = list(data)
for x in range(file_header_size,file_size):
    dst_data_lst[x] = 255 - data[x]
    pass

for x in dst_data_lst:
    newfile.write(x.to_bytes(1, 'little'))
    pass