cd asm
FOR %%i IN (test.asm 1E.asm) DO (
   ECHO %%i
   ca65 -o %%i.o %%i
   ld65 -C pic.cfg -o %%i.pic %%i.o
)
