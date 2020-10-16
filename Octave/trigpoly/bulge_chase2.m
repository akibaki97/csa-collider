function A = bulge_chase2(A,ro)
    n = length(A);

    if nargin == 1,
        ro = eig(A(n-1:n,n-1:n));
    endif

    x = eye(n,1);
    x(1) = A(1,1) - ro(1);
    x(2) = A(2,1);
    x = x / norm(x);
    x = A(1:3,1:3)*x(1:3) - ro(2)*x(1:3);
    x = x / norm(x);

    %printf("norm(y) = %s\n",disp(norm(y)));

    % calculate the reflector, which transforms y to a vector proportional to eye(n,1)
    [v,s] = get_householder(x);

    % apply the similarity transformation with it
    A(1:3,1:n) = A(1:3,1:n) - s * v * (v' * A(1:3,1:n));
    A(1:n,1:3) = A(1:n,1:3) - s * (A(1:n,1:3) * v) * v';

    % chase the bulge!
    m = 2;
    for k = 2:n-m,
        [v,s,alpha] = get_householder(A(k:k+m,k-1));

        % apply from left
        A(k:k+m,k-1:n) = A(k:k+m,k-1:n) - s * v * (v' * A(k:k+m,k-1:n));

        %apply from right
        A(1:n,k:k+m) = A(1:n,k:k+m) - s * (A(1:n,k:k+m) * v) * v';
        
        %printf("wtf %s\n",disp(norm(A(k:k+m,k-1)- eye(m+1,1)*alpha)));
    endfor

    [v,s,alpha] = get_householder(A(n-m+1:n,n-m));
    A(n-m+1:n,n-m:n) = A(n-m+1:n,n-m:n) - s * v * (v' * A(n-m+1:n,n-m:n));
    A(1:n,n-m+1:n) = A(1:n,n-m+1:n) - s * (A(1:n,n-m+1:n) * v) * v';
    

endfunction

function [v,s,alpha] = get_householder(x)

    norm_x = norm(x);
    if norm_x < eps,
        v = x; s = 0;
        alpha = 0;
        return
    endif

    x = x / norm_x;
    abs_x1 = abs(x(1));
    if abs_x1 > 1e-8, c = x(1) / abs_x1;
    else, c = 1;
    endif

    v = x * conj(c);
    v(1) += 1;
    s = 1/v(1);
    alpha = -norm_x*c;

endfunction