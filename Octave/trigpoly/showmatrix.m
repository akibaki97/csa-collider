function showmatrix(A)
figure(1)
pcolor(log(abs(A)+1.0e-32))
axis ij
shading interp
%shading faceted

