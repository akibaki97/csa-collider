function [v,d2] = polyhedron_closest_pt_to_origin(V,F)

    v = [Inf,Inf,Inf];
    d2 = Inf;
    m = length(F);
    for i = 1:m,
        y = V(F(i,:)',:);
        A = [(y(2,:) - y(1,:))',(y(3,:) - y(1,:))'];
        ATA = A' * A;
        if det(ATA) < eps, continue;
        endif

        x = -inv(ATA) * A' * y(1,:)';
        l = [1 - x(1) - x(2), x(1), x(2)];
        u = l * y;
        if dot(u,u) < d2,
            d2 = dot(u,u);
            v = u;
        endif
    endfor

endfunction