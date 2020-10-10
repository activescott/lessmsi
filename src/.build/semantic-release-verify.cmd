@echo off

IF DEFINED CHOCO_KEY (
    echo "CHOCO_KEY is defined"
    EXIT 0    
) ELSE (
    echo "CHOCO_KEY is NOT defined" 1>&2
    EXIT 1 
)
