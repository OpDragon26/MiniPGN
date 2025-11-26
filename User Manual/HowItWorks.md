This is an in-depth explanation about how MiniPGN works. If you just want to use it, see HowToUse.md
## The .mpgn file type
.mpgn files begin with the signature `4D 50 47 4E`, the ISO-8859-1 encoding for "MPGN"

Next is the version of the program used, for example `76 30 31 2E 30 30` for "v01.00"

Followed by details about how the PGNs were encoded. This includes information about what the program is expecting to decode, so that it can act accordingly. For example, whether any metadata, like the names of players or the time control, is stored with the games, or is it simply a list of games.

The length of this portion may change version to version, currently it's just two bytes, for example `53 49` "SI" for Standard encoding and Include metadata

File metadata

 - `S` - Standard
 - `F` - Fast
 - `O` - Over-optimized

 - `I` - Include metadata
 - `E` - Exclude metadata

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

Piece codes

- `010` knight
- `011` bishop
- `100` rook
- `101` queen
- `110` king

      
Bytes that cannot normally appear can be considered control characters

`11100111` Draw

`11110111` White won

`11101111` Black won

`11111111` Unspecified

## Handling game metadata

By game metadata, I mean all the information that can be found before a pgn in lichess database files or on chess.com when you share a game.

The tag pairs are usually stored as strings, in .mpgn files they're given a byte each

- `01` Not recognized
  - Followed by a null terminated string for name, then one for the value
- `02` Event
  - Followed by a byte signaling what comes after
    - `01` null terminated string
    - `02` "Live Chess" (chess.com)
    - `03` lichess rated game
      - Followed by another byte for the time control
      - `01` Bullet
      - `02` Blitz
      - `03` Classical
      - `04` Correspondence
- `03` Site
  - `01` null terminated string
  - `02` "Chess.com"
  - `03` "https://lichess.org/" expects a string of characters afterwards
- `04` Round
  - Expects 2 bytes
  - If the most significant bit is on, it's "?"
  - Otherwise the bytes are interpreted as an Int16
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
  - `01` Expects 4 bytes
    - 2 for time
    - 2 for bonus
    - if the bonus is 0, it's decoded as "+0", if it's negative then it's now shown at all
  - `02` expects a null-terminated string
- `0C` WhiteElo
  - 2 bytes
- `0D` BlackElo
  - 2 bytes
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
  - expects a null-terminated string
- `12` Terminaton
  - `01` null-terminated string
  - `02` normal
  - `03` time forfeit
  - `04` abandoned
  - `05` adjudication
  - `06` death
  - `07` emergency
  - `08` rules infraction
  - `09` unterminated
- `13` EndTime
  - Used by chess.com
  - Expects 4 bytes
    - hour
    - minute
    - second
    - GMT+x
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
- `19` SetUp
  - 2 bytes
- `FF` Begin game
  - No longer looks for tag pairs, instead starts parsing the next byte as a game




