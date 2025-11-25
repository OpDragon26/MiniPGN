This is an in-depth explanation about how MiniPGN works. If you just want to use it, see HowToUse.md
## The .mpgn file type
.mpgn files begin with the signature `4D 50 47 4E`, the ISO-8859-1 encoding for "MPGN"

Next is the version of the program used, for example `76 30 31 2E 30 30` for "v01.00"

Followed by details about how the PGNs were encoded. This includes information about what the program is expecting to decode, so that it can act accordingly. For example, whether any metadata, like the names of players or the time control, is stored with the games, or is it simply a list of games.

The length of this portion may change version to version, currently it's just two bytes, for example `53 49` "SI" for Standard encoding and Include metadata
## Move encoding
### Standard/default:
If the first (most significant) bit of the move is `0`, that means the move is a non-promoting pawn move

- If the second bit is `0`, either a pawn is moving forward to the square or capturing it from the left (from white's perspective) and if it's a `1`, a pawn capturing from the right
  - The following 6 bits represent the target square, totaling exactly 8 bits
  - The byte representation of both cxd5 and d5 would be `00 011 100` (these two moves are not possible on a board at the same time), and the byte representation of exd5 would be `01 011 100`.

If the first bit is `1`
- And the second is `0`, that means only one piece could move to the target square
  - The next 6 bits represent the target square for a total of 8 bits
  - Making sure that such moves only take up one byte instead of 2 doesn't seem like it helps much, however this would nearly halve the size of endgames
  - The byte representation of Qf7, if no other piece can move to the square, would be `10 101 110`

- If the second bit is `1`
  - And the 3rd bit is `0`, that means the move is a promotion, and the next 3 bits represent the piece being promoted to
    - In the second byte, since the files are always given, 3 bits represent the source file and 3 bits represent the target file. This is necessary for disambiguation
    - The byte representation of exd8=Q would be `110 101 00  100 011 00`

  - If the 3rd bit is `1` that means the move is a piece move, if multiple other pieces could move to the target square
    - The next 2 bits represent disambiguation
    - `00` means the move was not disambiguated
      - The next 3 bits represent the piece being moved
      - The 6 bits stored in the second byte represent the target square
      - The byte representation of Nc3 would be `111 00 010  010 010 00`
    - `10` file, `01` rank, `11` double
      - The next 3 bits represent the piece moved to make it easier to convert back to algebraic notation
      - The 2nd byte stores the source square
      - The 3rd byte stores the target square
      - The byte representation of Nb1c3 would be `111 11 010  001 000 00  010 010 00`

Control characters are bytes that cannot normally appear
The byte `11100111` would mean a move with an invalid piece

## Handling metadata
