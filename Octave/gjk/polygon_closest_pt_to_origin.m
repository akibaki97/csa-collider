function closest_v = polygon_closest_pt_to_origin(P)

    m = length(P);
    closest_v = P(1,:);
    for i = 1:m-1,
        y0 = P(i,:);
        y1 = P(i+1,:);
        dy = y1 - y0;
        denom = dot(dy,dy);

        if denom < eps, continue;
        endif

        t = dot(-dy,y0)/denom;
        if 0 < t && t < 1,
            v = y0 + t * dy;
            if dot(v,v) < dot(closest_v,closest_v),
                closest_v = v;
            endif
        end
    endfor
    
endfunction