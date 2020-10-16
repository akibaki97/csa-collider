function W = cso(U,V)

n = size(U)(1);
m = size(V)(1);

Y = zeros(n*m,2);
for i = 1:n,
    for j = 1:m,
        Y(j + (i-1)*m,:) = U(i,:) - V(j,:);
    endfor
endfor

E = convhull(Y);
N = size(E)(1);

W = zeros(N,2);

for k = 1:N,
    W(k,:) = Y(E(k),:);
endfor

end