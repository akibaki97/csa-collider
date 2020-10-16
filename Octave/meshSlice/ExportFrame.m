function [] = ExportFrame(fileName,V,F,n)

aabbMin = [min(V(:,1)),min(V(:,2)),min(V(:,3))];
aabbMax = [max(V(:,1)),max(V(:,2)),max(V(:,3))];

fileID = fopen(fileName,'w');
fprintf(fileID,'{\n');
fprintf(fileID,'\t"slices" : [\n');

normals = [1,0,0; 0,1,0; 0,0,1];

for d = 1:3,
    heights = linspace(aabbMin(d),aabbMax(d),n+2);
    for k = 2:n+1,
        fprintf(fileID,'\t\t{\n');
        fprintf(fileID,'\t\t\t"normal" : { "x" : %f, "y" : %f, "z" : %f },\n',normals(d,1),normals(d,2),normals(d,3));
        fprintf(fileID,'\t\t\t"refPoint" : { "x" : %f, "y" : %f, "z" : %f },\n',heights(k)*normals(d,1),heights(k)*normals(d,2),heights(k)*normals(d,3));
        fprintf(fileID,'\t\t\t"polygons": [\n');
        PE = MeshSlicer(V,F,heights(k),d);
        P = PolygonsFromEdges(PE);
        r = rows(P);
        for i = 1:r,
            fprintf(fileID,'\t\t\t\t{\n');
            fprintf(fileID,'\t\t\t\t\t"vertices" : [\n')
            [x,y] = FormatPolygon(P(i,:));
            m = length(x);
            for j = 1:m,
                fprintf(fileID,'\t\t\t\t\t\t{ "x" : %f, "y" : %f},\n',x(j),y(j));
            end
            fprintf(fileID,'\t\t\t\t\t\t{ "x" : %f, "y" : %f} \n',x(m),y(m));
            fprintf(fileID,'\t\t\t\t\t]\n')
            if i < r, fprintf(fileID,'\t\t\t\t},\n')
            else fprintf(fileID,'\t\t\t\t}\n');
            end
        end
        fprintf(fileID,'\t\t\t]\n');
        if k < n+1 || d < 3, fprintf(fileID,'\t\t},\n');
        else fprintf(fileID,'\t\t}\n');
        end
    end
end

fprintf(fileID,'\t]\n');
fprintf(fileID,'}');
fclose(fileID);

end