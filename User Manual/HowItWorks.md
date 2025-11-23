This is an in-depth explanation about how MiniPGN works. If you just want to use it, see HowToUse.md
## The .mpgn file type
.mpgn files begin with the signature `4D 50 47 4E`, the ISO-8859-1 encoding for "MPGN"

Next is the version of the program used, for example `76 30 31 2E 30 30` for "v01.00"

Followed by details about how the PGNs were encoded. This includes information about what the program is expecting to decode, so that it can act accordingly. For example, whether any metadata, like the names of players or the time control, is stored with the games, or is it simply a list of games.

The length of this portion may change version to version, currently it's just two bytes. `44 4E` for "DI": default for the version, and ignore metadata.
## Move encoding
### Standard/default:
If the first bit of the move is `0`, that means the move is a pawn move

&emsp;If the second bit is also `0`, the move is a single pawn move forward

&emsp;&emsp;The next 6 bits stand for the target square - 3 bits for the file and rank each, resulting in a total of 8 bits

&emsp;&emsp;`00 100 011` is the binary representation of `e4`

&emsp;If instead the second bit is `1`

&emsp;&emsp;And the 3rd bit is `0`, that means the move is a pawn capture

&emsp;&emsp;&emsp;A square can only be captured by a pawn from two files. The next bit acts as a disambiguator, even when there is no actual need for disambiguation.

&emsp;&emsp;&emsp;`0` means it's the file to the left (previous) and `1` means it's the one to the right (next)

&emsp;&emsp;&emsp;The next 6 bits represent the target square, resulting in a total of 9 bits

&emsp;&emsp;&emsp;En passant is treated as a regular capture

&emsp;&emsp;&emsp;`010 1 011 100` is the binary representation of `exd5`

&emsp;&emsp;If the 3rd bit is `1`, that means the move is a promotion

&emsp;&emsp;&emsp;The next 3 bits stand for the origin file, and the 2 after that represent which of the 3 squares seen by the pawn the promotion happens on. This is included regardless of whether disambiguation is needed or not.

&emsp;&emsp;&emsp;`11` means the promotion happens on the same file. `01` means it's on the file to the right, and `10` means it's on the file to the left

&emsp;&emsp;&emsp;Pawns can promote to 4 pieces, and the next 2 bits represent the piece, in the order of queen, knight, rook, bishop, for a total number of 10 bits.

&emsp;&emsp;&emsp;`011 110 10 00` is the binary representation of `gxf8=Q`

If the first bit is `1`, it means the move is a piece move

&emsp;The next 2 bits represent the disambiguation and the 3 after that represent the piece being moved,

&emsp;&emsp;`00`= no disambiguation, `01`= file disambiguation, `10`= rank disambiguation, `11`= double disambiguation

&emsp;&emsp;`000`= pawn, `001`= knight, `010`= bishop, `011`= rook, `100`= queen, `101`= king

&emsp;&emsp;If the move is double disambiguated, the piece can be omitted

&emsp;&emsp;After those, 6 bits are added for the target square

&emsp;&emsp;The total number of bytes is 12 for non-disambiguated moves and 15 for disambiguated moves
&emsp;&emsp;`1 00 001 110 110` is the binary representation of Ne5
&emsp;&emsp;`1 11 101 010 110 110` is the binary representation of Nf3e5

