function [w,ind] = support_mapping(V,d)
    plot_polygon(V);
    n = length(V);
    supportFound = false;
    maxProj = dot(V(1,:),d);
    i = 1;
    marked = zeros(n,1);
    marked(1) = true;
    hold on;
    plot(V(1,1),V(1,2),'po');
    while !supportFound,
        plot(V(i,1),V(i,2),'g*');
        pause(0.5)
        k = mod(i-2,n) + 1
        l = mod(i,n) + 1
        projL = dot(V(l,:),d);
        projK = dot(V(k,:),d);
        if !marked(l) && projL >= maxProj,
            i = l;
            maxProj = projL; 
            marked(i) = true;        
        elseif !marked(k) && projK >= maxProj,
            i = k;
            maxProj = projK;
            marked(i) = true;
        else
            supportFound = true;
            plot(V(i,1),V(i,2),'r*');
        endif
    endwhile
end