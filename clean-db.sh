#!/bin/sh
rm $(find db -not -name '*.sql' -not -name '.keep' -type f) > /dev/null 2>&1
