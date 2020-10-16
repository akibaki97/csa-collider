function [u,beta,alpha] = reflector(x,mode)
% Generate a reflector Q = eye - beta*u*u'
% such that Q*x = alpha*e_1 if mode == 1
% and       Q*x = alpha*e_n if mode == 2.

if mode == 2, m = size(x,1); else m = 1; end

% Rescale x so that x_1 is nonnegative and norm(x) = 1. 
scale = norm(x); 
if scale == 0
    u = x; beta = 0; alpha = 0;
else
    x = x/scale;
    if x(m) ~= 0
        phase = x(m)/abs(x(m));
        x = x*conj(phase); x(m) = real(x(m));
    else
        phase = 1;
    end

    % Build u and beta.
    u = x; u(m) = u(m) + 1;
    beta = 1/u(m);

    % Rescale
    alpha = -scale*phase;
end