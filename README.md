# CSharpDecodeSdpc
This is a tool to extract image tiles from pathological whole slide images (WSIs) based on C#.
The output tiles are in form of .png.

Authors: Hufei, Yiqing, Zhuobin
Institution: Tsinghua University, Shengqiang Technology Co., Ltd.

# Procedures

1.We use WSI-view software (https://www.sqray.com/yprj) to annotate the areas we want to extract. (Noted that these WSIs are in form of .sdpc which is a special form in Shengqiang Technology Co., Ltd.)
2.From the first step, we can get the annotation file with the suffix .sdpl. We put this annotation file (.sdpl) and the corresponding WSI (.sdpc) together. And use .py to process the annotation file. We only need to modify the input path and the output path. Then, we will obtain the new annotation file (-new.sdpl).
3.Run CSharpDecodeSdpc/a2/bin/Debug/a2.exe to output the expected tiles. These tiles are at the magnification of 40×.
