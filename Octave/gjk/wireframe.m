% plots the wireframe of a triangle mesh
% also super unefficient
function [] = wireframe(F,X,Y,Z,c)
    
    if !exist('c')
        c = rand(1,3) * 1/2 .+ 1/2;
    endif
    n = rows(F);
    hold on;
    for l = 1:n,
        i = F(l,1);
        j = F(l,2);
        k = F(l,3);
        plot3([X(i),X(j),X(k),X(i)],
              [Y(i),Y(j),Y(k),Y(i)],
              [Z(i),Z(j),Z(k),Z(i)],c);
    endfor

endfunction