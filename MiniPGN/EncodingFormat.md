This is an in-depth explanation about how MiniPGN works. If you just want to use it, see HowToUse.md
# The .mpgn file type
.mpgn files begin with the signature `4D 50 47 4E`, the ISO-8859-1 encoding for "MPGN"

Version of the algorithm used for encoding

`01 05`

Followed by details about how the PGNs were encoded. This includes information about what the program is expecting to decode, so that it can act accordingly. For example, whether any metadata, like the names of players or the time control, is stored with the games, or is it simply a list of games.

The length of this portion may change version to version, currently it's just two bytes, for example `53 49` "SI" for Standard encoding and Include metadata

### Encoding type

 - `S` - Standard
 - `F` - Fast

### Metadata treatment

 - `I` - Include metadata
 - `E` - Exclude metadata

### This is followed by a handful of optional file metadata tags

- `01` Date and time of encoding Unix time
  - unsigned 8 byte integer
- `02` Number of games
  - 8 byte unsigned integer

Metadata section ends with `FF` 

# Opening index

A series of null-terminated strings, each corresponding to an opening that appears in the metadata tags

The openings appear in the order they are present in the original file

Section ends with `FF`

# Move encoding
## Standard/default:
If the first (most significant) bit of the move is `0`, that means the move is a non-promoting pawn move

- If the second bit is `0`, either a pawn is moving forward to the square or capturing it from the left (from white's perspective) and if it's a `1`, a pawn capturing from the right
  - The following 6 bits represent the target square, totaling exactly 8 bits
  - The byte representation of both cxd5 and d5 would be `00_011_100` (these two moves are not possible on a board at the same time), and the byte representation of exd5 would be `01_011_100`.

If the first bit is `1`
- And the second is `0`, that means only one piece could move to the target square
  - The next 6 bits represent the target square for a total of 8 bits
  - Making sure that such moves only take up one byte instead of 2 doesn't seem like it helps much, however this would nearly halve the size of some endgames
  - The byte representation of Qf7, if no other piece can move to the square, would be `10_101_110`

- If the second bit is `1`
  - And the 3rd bit is `0`, that means the move is a promotion, and the last 3 bits represent the piece being promoted to
    - In the second byte, since the files are always given, 3 bits represent the source file and 3 bits represent the target file. This is necessary for disambiguation
    - The byte representation of exd8=Q would be `110_00_101  00_100_011`

  - If the 3rd bit is `1` that means the move is a piece move, if multiple other pieces could move to the target square
    - The next 2 bits represent disambiguation
    - `00` means the move was not disambiguated
      - The last 3 bits represent the piece being moved
      - The 6 bits stored in the second byte represent the target square
      - The byte representation of Nc3 would be `111_00_010  00_010_010`
    - `10` file, `01` rank, `11` double
      - The next 3 bits represent the piece moved to make it easier to convert back to algebraic notation
      - The 2nd byte stores the source square
      - The 3rd byte stores the target square
      - The byte representation of Nb1c3 would be `111_11_010  00_001_000  00_010_010`

### Piece codes

- `010` knight
- `011` bishop
- `100` rook
- `101` queen
- `110` king

### Bytes that cannot normally appear can be considered control characters

- `07` Draw
- `0F` White won
- `17` Black won
- `2F` White won by checkmate
- `37` Black won by checkmate
- `3F` Unspecified end of game
- `E7` %eval move tag
  - Followed by 4 bytes representing a float
- `EF` %eval checkmate tag
  - Followed by a byte representing the number of moves until mate
- `F7` Move eval suffix
  - `01` ??
  - `02` ?!
  - `03` !?
  - `04` !!
  - `05` ?
  - `06` !

# Handling game metadata

By game metadata, I mean all the information that can be found before a pgn in lichess database files or on chess.com when you share a game.

The tag pairs are usually stored as strings, in .mpgn files they're given a byte each

- `01` Byte count
  - Mandatory
  - Number of bytes between the beginning of this game to the beginning of the next one (tags included)
  - 2 byte unsigned integer
- `02` Event
  - Followed by a byte signaling what comes after
    - `01` null terminated string
    - `02` "Live Chess" (chess.com)
    - `03` Rated Bullet game
    - `04` Rated Blitz game
    - `05` Rated Classical game
    - `06` Rated Correspondence game
- `03` Site
  - `01` null terminated string
  - `02` "Chess.com"
  - `03` "https://lichess.org/" expects a string of characters afterwards
- `04` Round
  - `01` null terminated string
  - `02` next byte tells exact number
  - `03` "?"
- `05` White
  - null terminated string
- `06` Black
  - null terminated string
- `07` Result
  - `01` white won 1-0
  - `02` black won 0-1
  - `03` draw 1/2-1/2
- `08` Date
  - Used by chess.com
  - Expects 4 bytes
    - 2 for the year
    - 1 for month
    - 1 for day
- `09` UTCDate
  - Lichess equivalent of date
- `0A` UTCTime
  - Used by lichess
  - Expects 3 bytes
    - hour
    - minute
    - second
- `0B` TimeControl
  - `01` expects a null-terminated string
  - `02` Expects 4 bytes
    - 2 for time
    - 2 for bonus
    - if the bonus is 0, it's decoded as "+0", if it's `FF FF` then it's now shown at all
- `0C` WhiteElo
  - 2 bytes
  - `FF FF` "?"
- `0D` BlackElo
  - 2 bytes
  - `FF` "?"
- `0E` WhiteRatingDiff
  - Used by Lichess
  - 2 bytes
- `0F` BlackRatingDiff
  - Used by Lichess
  - 2 bytes
- `10` ECO code
  - expects 2 bytes
    - 1 for the letter
    - 1 for the number
- `11` Opening
  - Used by Lichess
  - `FF` null-terminated string
  - otherwise expects 2 bytes corresponding to an opening index
- `12` Terminaton
  - `01` null-terminated string
  - `02` Normal
  - `03` Time forfeit
  - `04` Abandoned
  - `05` Adjudication
  - `06` Death
  - `07` Emergency
  - `08` Rules infraction
  - `09` Unterminated
- `13` EndTime
  - Used by chess.com
  - Expects 5 bytes
    - hour
    - minute
    - second
    - 2 for GMT+x
- `14` Annotator
  - null-terminated string
- `15` PlyCount
  - expects 2 bytes
- `16` Time
  - same as UTCTime
- `17` Mode
  - `01` null-terminated string
  - `02` OTB
  - `03` ICS
- `18` FEN
  - expects a null-terminated string
- `19` WhiteTitle
  - `01` null terminated string
  - `02` GM
  - `03` IM
  - `04` FM
  - `05` CM
  - `06` WGM
  - `07` WIM
  - `08` WFM
  - `09` WCM
  - `0A` NM
  - `0B` SM
  - `0C` LM
- `1A` BlackTitle
  - Same as WhiteTitle
- `FE` Not recognized
  - Followed by a null terminated string for name, then one for the value
- `FF` Begin game
  - No longer looks for tag pairs, instead starts parsing the next byte as a game




