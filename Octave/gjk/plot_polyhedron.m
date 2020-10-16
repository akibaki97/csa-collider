function [] = plot_polyhedron(V,F,c)
    if exist('c'),
        wireframe(F,V(:,1),V(:,2),V(:,3),c);
    else
        wireframe(F,V(:,1),V(:,2),V(:,3));
    endif
endfunction