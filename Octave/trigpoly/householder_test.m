% disp("test1:\n");
% 
% A = 10*rand(6,6);
% alpha = norm(A(:,1),2);
% v = A(:,1) - alpha * eye(6,1);
% v = v / norm(v,2);
% 
% A
% alpha
% 
% for k = 1:6,
%     A(:,k) = householder(A(:,k),v,false);
% endfor
% 
% A
% 
disp("test2:")

n = 5;
A = 10*rand(n,n);

B = A;
eig(A)

for k = 1:n-2,
    y = A(k+1:n,k);
    if abs(y(1)) > 1e-8, c = y(1) / abs(y(1));
    else, c = 1;
    endif
    v = y / norm(y) * conj(c);
    v(1) += 1;
    alpha = -norm(y) / conj(c);

    % A(k+1:n,k:n) = A(k+1:n,k:n) - 1 / v(1) * v * (v' * A(k+1:n,k:n));
    % A(1:n,k+1:n) = A(1:n,k+1:n) - 1 / v(1) * (A(1:n,k+1:n) * v) * v';

    A(k+1:n,k) = alpha * eye(n-k,1);
    for i = k+1:n,
        A(k+1:n,i) = householder(A(k+1:n,i),v,1/v(1),false);
    endfor
    
    for i = 1:n,
        A(i,k+1:n) = householder(A(i,k+1:n),v,1/v(1),true);
    endfor
endfor

eig(A)
A
