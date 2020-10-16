function [w,i] = support_mapping2d(V,d)
    global DEBUG;
    DEBUG__ = false;

    if DEBUG__,
        plot_polygon(V);
    endif

    n = length(V);
    supportFound = false;
    maxProj = dot(V(1,:),d);
    i = 1;
    marked = zeros(n,1);
    marked(1) = true;
    hold on;
    
    
    if DEBUG__,
        plot(V(1,1),V(1,2),'po');
    endif
    
    while !supportFound,
        
        if DEBUG__,
            plot(V(i,1),V(i,2),'g*');
            pause(0.5)
        endif

        k = mod(i-2,n) + 1;
        l = mod(i,n) + 1;
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
            w = V(i,:);

        if DEBUG__,
            plot(w(1,1),w(1,2),'r*');
        endif
        endif
    endwhile
    hold off;
end