function [] = plot_simplex(W)
    n = rows(W);
    if n == 1,
        plot3(W(1,1),W(1,2),W(1,3));
    elseif n == 2,
        plot3([W(1,1),W(2,1)],[W(1,2),W(2,2)],[W(1,3),W(2,3)]);
    elseif n == 3,
        F = [ 
            1,2,3
        ];
        c = rand(3,1);
        trisurf(F,W(:,1),W(:,2),W(:,3),'facealpha',0.5,'facecolor',c);
    elseif n == 4,
        F = [
            1,2,3
            1,2,4
            1,3,4
            2,3,4
        ];
        c = rand(3,1);
        trisurf(F,W(:,1),W(:,2),W(:,3),'facealpha',0.5,'facecolor',c);
    endif
    
endfunction