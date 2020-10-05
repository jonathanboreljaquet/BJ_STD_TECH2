import string
import io, sys
import struct
import codecs
import functools
import os
import re

class ID3:
  FRAME_ID_TO_FIELD = {  # Mapping from Frame ID to a field of the TinyTag
    'COMM': 'comment', 'COM': 'comment',
    'TRCK': 'track',  'TRK': 'track',
    'TYER': 'year',   'TYE': 'year', 'TDRC': 'year',
    'TALB': 'album',  'TAL': 'album',
    'TPE1': 'artist', 'TP1': 'artist',
    'TIT2': 'title',  'TT2': 'title',
    'TCON': 'genre',  'TCO': 'genre', 'TSSE' : 'encodersettings',
    'TPOS': 'disc', 'TKEY': 'key', 'TENC': 'encodedby', 'TBPM': 'bpm', 
    'TPE2': 'albumartist', 'TCOM': 'composer', 'TPUB': 'publisher',
  }
  IMAGE_FRAME_IDS = {'APIC', 'PIC'}
  PARSABLE_FRAME_IDS = set(FRAME_ID_TO_FIELD.keys()).union(IMAGE_FRAME_IDS)
  _MAX_ESTIMATION_SEC = 30
  _CBR_DETECTION_FRAME_COUNT = 5
  _USE_XING_HEADER = True  # much faster, but can be deactivated for testing

  ID3V1_GENRES = [
    'Blues', 'Classic Rock', 'Country', 'Dance', 'Disco',
    'Funk', 'Grunge', 'Hip-Hop', 'Jazz', 'Metal', 'New Age', 'Oldies',
    'Other', 'Pop', 'R&B', 'Rap', 'Reggae', 'Rock', 'Techno', 'Industrial',
    'Alternative', 'Ska', 'Death Metal', 'Pranks', 'Soundtrack',
    'Euro-Techno', 'Ambient', 'Trip-Hop', 'Vocal', 'Jazz+Funk', 'Fusion',
    'Trance', 'Classical', 'Instrumental', 'Acid', 'House', 'Game',
    'Sound Clip', 'Gospel', 'Noise', 'AlternRock', 'Bass', 'Soul', 'Punk',
    'Space', 'Meditative', 'Instrumental Pop', 'Instrumental Rock',
    'Ethnic', 'Gothic', 'Darkwave', 'Techno-Industrial', 'Electronic',
    'Pop-Folk', 'Eurodance', 'Dream', 'Southern Rock', 'Comedy', 'Cult',
    'Gangsta', 'Top 40', 'Christian Rap', 'Pop/Funk', 'Jungle',
    'Native American', 'Cabaret', 'New Wave', 'Psychadelic', 'Rave',
    'Showtunes', 'Trailer', 'Lo-Fi', 'Tribal', 'Acid Punk', 'Acid Jazz',
    'Polka', 'Retro', 'Musical', 'Rock & Roll', 'Hard Rock'
    # Wimamp Extended Genres
    'Folk', 'Folk-Rock', 'National Folk', 'Swing', 'Fast Fusion', 'Bebob',
    'Latin', 'Revival', 'Celtic', 'Bluegrass', 'Avantgarde', 'Gothic Rock',
    'Progressive Rock', 'Psychedelic Rock', 'Symphonic Rock', 'Slow Rock',
    'Big Band', 'Chorus', 'Easy Listening', 'Acoustic', 'Humour', 'Speech',
    'Chanson', 'Opera', 'Chamber Music', 'Sonata', 'Symphony', 'Booty Bass',
    'Primus', 'Porn Groove', 'Satire', 'Slow Jam', 'Club', 'Tango', 'Samba',
    'Folklore', 'Ballad', 'Power Ballad', 'Rhythmic Soul', 'Freestyle',
    'Duet', 'Punk Rock', 'Drum Solo', 'A capella', 'Euro-House',
    'Dance Hall', 'Goa', 'Drum & Bass'
    # according to https://de.wikipedia.org/wiki/Liste_der_ID3v1-Genres:
    'Club-House', 'Hardcore Techno', 'Terror', 'Indie', 'BritPop',
    '',  # don't use ethnic slur ("Negerpunk", WTF!)
    'Polsk Punk', 'Beat', 'Christian Gangsta Rap', 'Heavy Metal',
    'Black Metal', 'Contemporary Christian', 'Christian Rock',
    # WinAmp 1.91
    'Merengue', 'Salsa', 'Thrash Metal', 'Anime', 'Jpop', 'Synthpop',
    # WinAmp 5.6
    'Abstract', 'Art Rock', 'Baroque', 'Bhangra', 'Big Beat', 'Breakbeat',
    'Chillout', 'Downtempo', 'Dub', 'EBM', 'Eclectic', 'Electro',
    'Electroclash', 'Emo', 'Experimental', 'Garage', 'Illbient',
    'Industro-Goth', 'Jam Band', 'Krautrock', 'Leftfield', 'Lounge',
    'Math Rock', 'New Romantic', 'Nu-Breakz', 'Post-Punk', 'Post-Rock',
    'Psytrance', 'Shoegaze', 'Space Rock', 'Trop Rock', 'World Music',
    'Neoclassical', 'Audiobook', 'Audio Theatre', 'Neue Deutsche Welle',
    'Podcast', 'Indie Rock', 'G-Funk', 'Dubstep', 'Garage Rock', 'Psybient',
  ]
  # see this page for the magic values used in mp3:
  # http://www.mpgedit.org/mpgedit/mpeg_format/mpeghdr.htm
  samplerates = [
    [11025, 12000,  8000],  # MPEG 2.5
    [],                     # reserved
    [22050, 24000, 16000],  # MPEG 2
    [44100, 48000, 32000],  # MPEG 1
  ]

  v1l1 = [0, 32, 64, 96, 128, 160, 192, 224, 256, 288, 320, 352, 384, 416, 448, 0]
  v1l2 = [0, 32, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320, 384, 0]
  v1l3 = [0, 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320, 0]
  v2l1 = [0, 32, 48, 56, 64, 80, 96, 112, 128, 144, 160, 176, 192, 224, 256, 0]
  v2l2 = [0, 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 144, 160, 0]
  v2l3 = v2l2

  samples_per_frame = 1152  # the default frame size for mp3
  bitrate_by_version_by_layer = [
      [None, v2l3, v2l2, v2l1],  # MPEG Version 2.5  # note that the layers go
      None, # reserved          # from 3 to 1 by design.
      [None, v2l3, v2l2, v2l1],  # MPEG Version 2    # the first layer id is
      [None, v1l3, v1l2, v1l1],  # MPEG Version 1    # reserved
  ]

  samplerates = [
    [11025, 12000,  8000],  # MPEG 2.5
    [],                     # reserved
    [22050, 24000, 16000],  # MPEG 2
    [44100, 48000, 32000],  # MPEG 1
  ]

  @staticmethod
  def _parse_xing_header(fh):
      # see: http://www.mp3-tech.org/programmer/sources/vbrheadersdk.zip
      fh.seek(4, os.SEEK_CUR)  # read over Xing header
      header_flags = struct.unpack('>i', fh.read(4))[0]
      frames = byte_count = toc = vbr_scale = None
      if header_flags & 1:  # FRAMES FLAG
          frames = struct.unpack('>i', fh.read(4))[0]
      if header_flags & 2:  # BYTES FLAG
          byte_count = struct.unpack('>i', fh.read(4))[0]
      if header_flags & 4:  # TOC FLAG
          toc = [struct.unpack('>i', fh.read(4))[0] for _ in range(100)]
      if header_flags & 8:  # VBR SCALE FLAG
          vbr_scale = struct.unpack('>i', fh.read(4))[0]
      return frames, byte_count, toc, vbr_scale


class TagException(LookupError):  # inherit LookupError for backwards compat
  pass

class mp3Main:
  def __init__(self, FileName):
    self.FileName = FileName
    self.mpFile = mp3File(self.FileName)
  def getFileName(self):
    return self.FileName

class mp3File:
  def __init__(self, fileName):
    self.fileName = fileName
    self.bytepos_after_id3v2 = 0
    self.ignore_errors = False
    self.load_image = False
    self.fieldname = None
    self.filesize = os.path.getsize(self.fileName)
    self.album = None
    self.albumartist = None
    self.artist = None
    self.audio_offset = None
    self.bitrate = None
    self.bpm = None
    self.channels = None
    self.comment = None
    self.composer = None
    self.disc = None
    self.disc_total = None
    self.duration = None
    self.encodedby = None
    self.encodersettings = None
    self.genre = None
    self.key = None
    self.publisher = None
    self.samplerate = None
    self.title = None
    self.track = None
    self.track_total = None
    self.year = None
    self._load_image = False
    self._image_data = None
    self._ignore_errors = False
    self.channels_per_channel_mode = [
        2,  # 00 Stereo
        2,  # 01 Joint stereo (Stereo)
        2,  # 10 Dual channel (2 mono channels)
        1,  # 11 Single channel (Mono)
    ]
    self.sizeadd = 0
    try:
      self.file = open(fileName, "rb")
    except IOError as msg:
        print("Can't open %s: %s" % (self.fileName, msg))
    
    self.getHeader()
    self._determine_duration(self.file)

  def _determine_duration(self, fh):
    max_estimation_frames = (ID3._MAX_ESTIMATION_SEC * 44100) // ID3.samples_per_frame
    frame_size_accu = 0
    header_bytes = 4
    frames = 0  # count frames for determining mp3 duration
    bitrate_accu = 0    # add up bitrates to find average bitrate to detect
    last_bitrates = []  # CBR mp3s (multiple frames with same bitrates)
    # seek to first position after id3 tag (speedup for large header)
    fh.seek(self.bytepos_after_id3v2)
    while True:
        # reading through garbage until 11 '1' sync-bits are found
        b = fh.peek(4)
        if len(b) < 4:
            break  # EOF
        sync, conf, bitrate_freq, rest = struct.unpack('BBBB', b[0:4])
        br_id = (bitrate_freq >> 4) & 0x0F  # biterate id
        sr_id = (bitrate_freq >> 2) & 0x03  # sample rate id
        padding = 1 if bitrate_freq & 0x02 > 0 else 0
        mpeg_id = (conf >> 3) & 0x03
        layer_id = (conf >> 1) & 0x03
        channel_mode = (rest >> 6) & 0x03
        # check for eleven 1s, validate bitrate and sample rate
        if not b[:2] > b'\xFF\xE0' or br_id > 14 or br_id == 0 or sr_id == 3 or layer_id == 0 or mpeg_id == 1:
            idx = b.find(b'\xFF', 1)  # invalid frame, find next sync header
            if idx == -1:
                idx = len(b)  # not found: jump over the current peek buffer
            fh.seek(max(idx, 1), os.SEEK_CUR)
            continue
        try:
            self.channels = self.channels_per_channel_mode[channel_mode]
            frame_bitrate = ID3.bitrate_by_version_by_layer[mpeg_id][layer_id][br_id]
            self.samplerate = ID3.samplerates[mpeg_id][sr_id]
        except (IndexError, TypeError):
            raise TagException('mp3 parsing failed')
        # There might be a xing header in the first frame that contains
        # all the info we need, otherwise parse multiple frames to find the
        # accurate average bitrate
        if frames == 0 and ID3._USE_XING_HEADER:
            xing_header_offset = b.find(b'Xing')
            if xing_header_offset != -1:
                fh.seek(xing_header_offset, os.SEEK_CUR)
                xframes, byte_count, toc, vbr_scale = ID3._parse_xing_header(fh)
                if xframes and xframes != 0 and byte_count:
                    self.duration = xframes * ID3.samples_per_frame / float(self.samplerate)
                    self.bitrate = int(byte_count * 8 / self.duration / 1000)
                    self.audio_offset = fh.tell()
                    return
                continue 
        frames += 1  # it's most probably an mp3 frame
        bitrate_accu += frame_bitrate
        if frames == 1:
            self.audio_offset = fh.tell()
        if frames <= ID3._CBR_DETECTION_FRAME_COUNT:
            last_bitrates.append(frame_bitrate)
        fh.seek(4, os.SEEK_CUR)  # jump over peeked byte  
        frame_length = (144000 * frame_bitrate) // self.samplerate + padding
        frame_size_accu += frame_length
        # if bitrate does not change over time its probably CBR
        is_cbr = (frames == ID3._CBR_DETECTION_FRAME_COUNT and
                  len(set(last_bitrates)) == 1)
        if frames == max_estimation_frames or is_cbr:
            # try to estimate duration
            fh.seek(-128, 2)  # jump to last byte (leaving out id3v1 tag)
            audio_stream_size = fh.tell() - self.audio_offset
            est_frame_count = audio_stream_size / (frame_size_accu / float(frames))
            samples = est_frame_count * ID3.samples_per_frame
            self.duration = samples / float(self.samplerate)
            self.bitrate = int(bitrate_accu / frames)
            return 
        if frame_length > 1:  # jump over current frame body
            fh.seek(frame_length - header_bytes, os.SEEK_CUR)
    if self.samplerate:
        self.duration = frames * ID3.samples_per_frame / float(self.samplerate)


  def read(self,fh, nbytes):  # helper function to check if we haven't reached EOF
    b = fh.read(nbytes)
    if len(b) < nbytes:
      raise TagException('Unexpected end of file')
    return b
  
  def calc_size(self, bytestr, bits_per_byte):
    # length of some mp3 header fields is described by 7 or 8-bit-bytes
    return functools.reduce(lambda accu, elem: (accu << bits_per_byte) + elem, bytestr, 0)

  def unpad(self,s):
    # strings in mp3 and asf *may* be terminated with a zero byte at the end
    return s.replace('\x00', '')
  
  def decode_string(self, b):
    try:  # it's not my fault, this is the spec.
      first_byte = b[:1]
      if first_byte == b'\x00':  # ISO-8859-1
        bytestr = b[1:]
        encoding = 'ISO-8859-1'
      elif first_byte == b'\x01':  # UTF-16 with BOM
        # read byte order mark to determine endianess
        encoding = 'UTF-16be' if b[1:3] == b'\xfe\xff' else 'UTF-16le'
        # strip the bom and optional null bytes
        bytestr = b[3:-1] if len(b) % 2 == 0 else b[3:]
      elif first_byte == b'\x02':  # UTF-16LE
        # strip optional null byte, if byte count uneven
        bytestr = b[1:-1] if len(b) % 2 == 0 else b[1:]
        encoding = 'UTF-16le'
      elif first_byte == b'\x03':  # UTF-8
        bytestr = b[1:]
        encoding = 'UTF-8'
      else:
        bytestr = b
        encoding = 'ISO-8859-1'  # wild guess
      if bytestr[:4] == b'eng\x00':
        bytestr = bytestr[4:]  # remove language
      errors = 'ignore' if self.ignore_errors else 'strict'
      return self.unpad(codecs.decode(bytestr, encoding, errors))
    except UnicodeDecodeError:
      raise TagException('Error decoding ID3 Tag!')


  def parse_frame(self, fh, id3version=False):
    # ID3v2.2 especially ugly. see: http://id3.org/id3v2-00
    frame_header_size = 6 if id3version == 2 else 10

    frame_size_bytes = 3 if id3version == 2 else 4
    
    binformat = '3s3B' if id3version == 2 else '4s4B2B'
    
    bits_per_byte = 7 if id3version == 4 else 8  # only id3v2.4 is synchsafe
    
    frame_header_data = fh.read(frame_header_size)
    
    if len(frame_header_data) != frame_header_size:
      return 0
    frame = struct.unpack(binformat, frame_header_data)
    frame_id = self.decode_string(frame[0])
    frame_size = self.calc_size(frame[1:1+frame_size_bytes], bits_per_byte)

    self.sizeadd += 10 
    self.sizeadd += frame_size
 
    if frame_size > 0:
      print(frame_id)
    # flags = frame[1+frame_size_bytes:] # dont care about flags.
      if frame_id not in ID3.PARSABLE_FRAME_IDS:  # jump over unparsable frames
        fh.seek(frame_size, os.SEEK_CUR)
        return frame_size
      content = fh.read(frame_size)
      self.fieldname = ID3.FRAME_ID_TO_FIELD.get(frame_id)
      print(frame_id, ' : ',  self.fieldname)
      if self.fieldname:
        self.set_field(self.fieldname, content, self.decode_string)
      elif frame_id in ID3.IMAGE_FRAME_IDS and self.load_image:
        # See section 4.14: http://id3.org/id3v2.4.0-frames
        if frame_id == 'PIC':  # ID3 v2.2:
          desc_end_pos = content.index(b'\x00', 1) + 1
        else:  # ID3 v2.3+
          mimetype_end_pos = content.index(b'\x00', 1) + 1
          desc_start_pos = mimetype_end_pos + 1  # jump over picture type
          desc_end_pos = content.index(b'\x00', desc_start_pos) + 1
        if content[desc_end_pos:desc_end_pos+1] == b'\x00':
          desc_end_pos += 1  # the description ends with 1 or 2 null bytes
          self.image_data = content[desc_end_pos:]
      return frame_size
    return 0
  
  def set_field(self, fieldname, bytestring, transfunc=None):
    """convienience function to set fields of the tinytag by name.
    the payload (bytestring) can be changed using the transfunc"""
    if getattr(self, self.fieldname):  # do not overwrite existing data
      return
    value = bytestring if transfunc is None else transfunc(bytestring)
    if fieldname == 'genre':
      if value.isdigit() and int(value) < len(ID3.ID3V1_GENRES):
        # funky: id3v1 genre hidden in a id3v2 field
        value = ID3.ID3V1_GENRES[int(value)]
      else:  # funkier: the TCO may contain genres in parens, e.g. '(13)'
        genre_in_parens = re.match('^\\((\\d+)\\)$', value)
        if genre_in_parens:
          value = ID3.ID3V1_GENRES[int(genre_in_parens.group(1))]
    if fieldname in ("track", "disc"):
      if type(value).__name__ in ('str', 'unicode') and '/' in value:
        current, total = value.split('/')[:2]
        setattr(self, "%s_total" % fieldname, total)
      else:
        current = value
      setattr(self, fieldname, current)
    else:
      setattr(self, fieldname, value)

  def getHeader(self):
    header = struct.unpack('3sBBB4B', self.read(self.file, 10))
    tag = codecs.decode(header[0], 'ISO-8859-1')
    if tag == 'ID3':
      maj, rev = header[1:3]
      print('ID3 V2.', maj,'.' ,rev)
      extended = (header[3] & 0x40) > 0
      if(extended):
        print('extended->True')
      else:
        print('extended->Flase')
      size = self.calc_size(header[4:8], 7)
      print(size)
      self.bytepos_after_id3v2 = size
      end_pos = self.file.tell() + size
      print(end_pos)
      parsed_size = 0
      if extended:  # just read over the extended header.
        size_bytes = struct.unpack('4B', self.read(self.file, 6)[0:4])
        extd_size = self.calc_size(size_bytes, 7)
        self.file.seek(extd_size - 6, os.SEEK_CUR)  # jump over extended_header
      while parsed_size < size:
        frame_size = self.parse_frame(self.file, id3version=maj)
        if frame_size == 0:
          break
        parsed_size += frame_size
      self.file.seek(end_pos, os.SEEK_SET)

if __name__ == "__main__":
  m = mp3Main("/Users/JO-PC/Desktop/BJ_STD_TECH2/BJ_PA_Exercice02/BJ_PA_Exercice02/song.mp3")
  print('album :', m.mpFile.album)
  print('albumartist :', m.mpFile.albumartist)
  print('artist :', m.mpFile.artist)
  print('audio_offset :', m.mpFile.audio_offset)
  print('bitrate :', m.mpFile.bitrate)
  print('bpm :', m.mpFile.bpm)
  print('channels :', m.mpFile.channels)
  print('comment :', m.mpFile.comment)
  print('composer :', m.mpFile.composer)
  print('disc :', m.mpFile.disc)
  print('disc_total :', m.mpFile.disc_total)
  print('duartion :', m.mpFile.duration)
  print('encoded by :', m.mpFile.encodedby)
  print('encodersettings :', m.mpFile.encodersettings)
  print('genre :', m.mpFile.genre)
  print('key :', m.mpFile.key)
  print('publisher :', m.mpFile.publisher)
  print('samplerate :', m.mpFile.samplerate)
  print('title :', m.mpFile.title)
  print('track :', m.mpFile.track)
  print('track_total :', m.mpFile.track_total)
  print('year :', m.mpFile.year)
  print('added :', m.mpFile.sizeadd)
