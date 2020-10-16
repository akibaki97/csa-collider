function A = makeAdjJson(filename)

[V,F] = ReadObjMesh(filename);

% build adjacencies into A
n = size(V)(1);
A = zeros(size(V)(1));
m = size(F)(1);
for k = 1:m,
    A(F(k,1),F(k,2)) = 1;
    A(F(k,1),F(k,3)) = 1;
    A(F(k,2),F(k,1)) = 1;
    A(F(k,2),F(k,3)) = 1;
    A(F(k,3),F(k,1)) = 1;
    A(F(k,3),F(k,2)) = 1;
endfor

N = length(filename);
jsonfilename = [filename(1:(N-4)),"_adj.json"];
fileId = fopen(jsonfilename,'w');
fprintf(fileId,'{\n');
fprintf(fileId,'\t"vertices" : [\n');
for k = 1:n,
    %fprintf(fileId,'\t\t{"x": %f,\n"y": %f,\n"z": %f,\n"adj ',V(k,1),V(k,2),V(k,3));
    fprintf(fileId,'\t\t{\n');
    fprintf(fileId,'\t\t\t"x" : %f, "y" : %f, "z" : %f,\n',V(k,1),V(k,2),V(k,3));
    fprintf(fileId,'\t\t\t"adj" : [');
    first = true;
    for l = 1:n,
        if A(k,l) == 1,
            if first, fprintf(fileId,'%d',l-1); first = false;
            else fprintf(fileId,', %d',l-1);
            endif
        endif
    endfor
    fprintf(fileId,']\n');

    if k < n,
        fprintf(fileId,'\t\t},\n');
    else
        fprintf(fileId,'\t\t}\n');
    endif
endfor

fprintf(fileId,'\t]\n');
fprintf(fileId,'}');
fclose(fileId);

%fprintf(fileId,'\t],\n')
%fprintf(fileId,'\t\n"adjacencies" : [\n');
%for k = 1:n,
%    fprintf(fileId,'\t\t[');
%    first = true;
%    for l = 1:n,
%        if A(k,l) == 1,
%            if first, fprintf(fileId,'%d',l-1); first = false;
%            else fprintf(fileId,', %d',l-1);
%            endif
%        endif
%    endfor
%    
%    if k < n, fprintf(fileId,'],\n');
%    else fprintf(fileId,']\n');
%    endif
%endfor
%fprintf(fileId,'\t]\n');
%fprintf(fileId,'}');
%fclose(fileId);

end