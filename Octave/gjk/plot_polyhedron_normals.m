function plot_polyhedron_normals(V,F,col)

    m = length(F);
    hold on;
    for i = 1:m,
        v1 = V(F(i,1),:);
        v2 = V(F(i,2),:);
        v3 = V(F(i,3),:);
        n = cross(v2-v1,v3-v1);
        n = n / norm(n);
        c = (v1 + v2 + v3)/3;
        cn = c + n;
        plot3(c(1),c(2),c(3),[col,'o']);
        plot3([c(1),cn(1)],[c(2),cn(2)],[c(3),cn(3)],col);
    endfor

end