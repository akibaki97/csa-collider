function export_obj(file_name,V,F)
    
    file_id = fopen(file_name,'w');
    obj_name = file_name(1:end-4);

    fprintf(file_id,"# Exported from Octave code\n");
    fprintf(file_id,'o %s\n',obj_name);

    for i = 1:length(V),
        fprintf(file_id,"v %f %f %f\n",V(i,1),V(i,2),V(i,3));
    endfor

    fprintf(file_id,"s off\n");

    for i = 1:length(F),
        fprintf(file_id,"f %d %d %d\n",F(i,1),F(i,2),F(i,3));
    endfor

    fclose(file_id);
end