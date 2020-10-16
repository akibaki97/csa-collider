function trigpolytest
    
    % Kiertekelesek
    N = 500;
    
    % Trigonometrikus polinom foka
    n = 5;
    x = linspace(-pi,pi,N)';
    
    % Egyutthatok
    ak = (2 * rand(1,n) - 1).* linspace(1,1/n,n);
    bk = (2 * rand(1,n) - 1).* linspace(1,1/n,n);
    a0 = 2 * rand;
    
    % Trigonometrikus polinom
    y = trigpoly(a0,ak,bk,x);
    

    % Komplex algebrai polinom egyutthatoi
    dk = trig2alg(a0,ak,bk);
    
    % Algebrai polinom
    k = 0:2*n;    
    Q = sum(dk .* exp(I * x * k),2);
    
    % Visszateres a trigonometrikus polinomra 
    % (az imaginarius resz numerikus szemettol eltekintve 0)
    Y = real(exp(-I * n * x) .* Q);
    
    % Polinom egyutthato vektor megforditva
    p = dk(end:-1:1);
    
    % Kisero (companion) matrix
    C  =  diag(ones(2 * n - 1,1), - 1);
    C(:,2*n) = -p(2*n+1:-1:2) ./ p(1);

    % Sajatertekek  => algebrai polinom komplex gyokei
    global num_it;

    % tic;
    % num_it = 0;
    % R = implicit_qr(C,false);
    % num_it
    % toc
    % tic;
    % global num_it;
    % num_it = 0;
    % R = implicit_qr(C,true);
    % num_it
    % toc
    transpose(p)
    R = aberth(p);

    %[transpose(R), 1 ./ R', abs(R')]

    % Az egysegkorre eso gyokok
    eps = 1e-8;
    Ind = abs(abs(R) - 1) < eps;
    rx = real(R(Ind));
    ry = imag(R(Ind));
    
    % A polarszog a [-pi,pi]-beli parameter, amit keresunk
    r = atan2(ry,rx);
    
    % Trigonometrikus nezet
    subplot(211);
    plot(x, 0 * x, 'k--');
    hold on
    plot(x,y,'b-');
    plot(x,Y,'r--');
    plot(r,0 * r,'r*');
    hold off
    
    
    % Komplex algebrai nezet
    subplot(212);          
    plot(cos(x), sin(x));
    hold on 
    
    % Wow! A gyokok szimmetrikusak
    % az egysegkorre!
    RR = 1./conj(R);
    plot(real(RR),imag(RR),'g*');
    plot(real(R),imag(R),'ro');
    plot(rx,ry,'r*');
    %axis([-3,3,-3,3])
    axis equal;
    hold off
end


% Trigonometrikus polinom kiertekelese
function y = trigpoly(a0,ak,bk,x)
    ak = ak(:)';
    bk = bk(:)';
    x = x(:);
    n = length(ak);
    k = 1:n;
    y = a0/2 + sum(ak .* cos(x * k) + bk .* sin(x * k),2);
end


% Trigonometrikus polinomhoz tartozo
% komplex algebrai polinom egyutthatoinak
% meghatarozasa
function dk = trig2alg(a0,ak,bk)
    n = length(ak);
    ak = ak(:)';
    bk = bk(:)';
    ck = (ak - I * bk)/2;
    cmk = conj(ck); 
    cmk = cmk(end:-1:1);
    c0 = a0/2;
    dk = [cmk,c0,ck];
end