"""Read-only binary inspection: python3 verify_game_formatter.py <TransportFever3.exe>.

Uses objdump and the switch offsets of the reviewed installed game build. This is static
corroboration of the separate Windows runtime test; it does not execute the game.
"""
import hashlib
import re
import struct
import subprocess
import sys
from pathlib import Path

file = Path(sys.argv[1])
data = file.read_bytes()
pe = struct.unpack_from('<I', data, 0x3c)[0]
count = struct.unpack_from('<H', data, pe + 6)[0]
optional_size = struct.unpack_from('<H', data, pe + 20)[0]
image_base = struct.unpack_from('<Q', data, pe + 48)[0]
sections = []
for index in range(count):
    offset = pe + 24 + optional_size + index * 40
    name = data[offset:offset+8].rstrip(b'\0').decode()
    _, rva, size, raw = struct.unpack_from('<IIII', data, offset + 8)
    sections.append((name, rva, size, raw))

def offset_for(rva):
    _, start, size, raw = next(section for section in sections if section[1] <= rva < section[1]+section[2])
    return raw + rva - start

string_offset = data.index(b'not a number in proper range')
_, section_rva, _, section_raw = next(section for section in sections if section[3] <= string_offset < section[3]+section[2])
string_rva = section_rva + string_offset - section_raw
references = []
for name, rva, size, raw in sections:
    if name != '.text':
        continue
    code = data[raw:raw+size]
    for match in re.finditer(rb'[\x48\x4c]\x8d[\x05\x0d\x15\x1d\x25\x2d\x35\x3d]', code):
        position = match.start()
        displacement = struct.unpack_from('<i', code, position + 3)[0]
        if rva + position + 7 + displacement == string_rva:
            references.append(image_base + rva + position)
assert len(references) == 1
address = references[0]
print('Executable SHA-256:', hashlib.sha256(data).hexdigest())
print('Range-error reference:', hex(address))
for character in ['d', 'i']:
    index = data[offset_for(0x2fd967c) + ord(character) - ord('A')]
    target = struct.unpack_from('<I', data, offset_for(0x2fd9660) + index * 4)[0]
    print('%' + character, 'switch target:', hex(image_base + target))
    assert image_base + target == 0x142fd924a
result = subprocess.run([
    'objdump', '-d', '--start-address=' + hex(address-350),
    '--stop-address=' + hex(address+420), str(file),
], capture_output=True, text=True, check=True)
assert 'vcvttsd2si %xmm0,%ebx' in result.stdout
print(result.stdout)
