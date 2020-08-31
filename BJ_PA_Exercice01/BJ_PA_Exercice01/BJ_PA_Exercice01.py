#Author : Jonathan Borel-Jaquet
#Date : 31/08/2020
#Description : Program for reading the header of a bmp file as well as its modification

import struct
import os
filename = "invert.bmp"
file = open(filename,"rb")
data = file.read()

#Read the file type
file_type = data[:0x02].decode("utf-8")
print(f"Signature du fichier : {file_type}")

#Read the the total size of the file in bytes
file_size = int.from_bytes(data[0x02:0x06],"little")
print(f"Taille totale du fichier en octets : {file_size}")

#Read the width image
file_width = int.from_bytes(data[0x12:0x16],"little")
print(f"Largeur image : {file_width}")

#Read the height image
file_height = int.from_bytes(data[0x12:0x16],"little")
print(f"Hauteur image : {file_height}")

#Read the number of bits per pixel
file_nbr_pixel = int.from_bytes(data[0x1C:0x1E],"little")
print(f"Nombre de bits par pixel : {file_nbr_pixel}")

#Read the number of bits per pixel
file_size_img = int.from_bytes(data[0x22:0x26],"little")
print(f"Taille en octets des données de l’image : {file_size_img}")

#invert color of the image
newfile = open("revert.bmp","wb")
newfile.write(data[:0x36])

for i in range(0x37,file_size,3):
	blue = int.from_bytes(data[i:i + 1],"little")
	green = int.from_bytes(data[i + 1:i + 2],"little")
	red = int.from_bytes(data[i + 2:i + 3],"little")
	
	new_blue = 255 - blue
	new_green = 255 - green
	new_red = 255 - red
	new_rgb_pixel = new_blue+new_green+new_red
	newfile.write(new_rgb_pixel.to_bytes(file_nbr_pixel, 'little'))

file.close()
newfile.close()
	






