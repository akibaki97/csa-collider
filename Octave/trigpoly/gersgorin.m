% nev: Rabely Akos 
% neptun: o6k9v7
%
% HFP1
% Adott negyzetes matrix eseten kirajzolja a sorokhoz illetve oszlopokhoz
% tartozo Gersgorin koroket (kulonbozo szinekkel).
% Egy kor kirajzolasa utan billentyunyomasra var a kovetkezo kor
% kirajzolasaig.
%
% Bemeno parameter: A: (negyzetes) matrix
% Kimeno parameter: kp: kozeppontok,
%        r_sor, r_oszl: Gersgorin sugarak.
% Rajz: sorokhoz tartozo korok: kekkel,
%       oszlopokhoz tartozo korok: pirossal.

function [kp,r_sor,r_oszl] = gersgorin(A)

% matrix negyzetessegenek ellenorzese
n = size(A);
if n(1) ~= n(2),
    error('Nem negyzetes matrix.')
end

n =n(1);

% kimeneti valtozok letrehozasa
kp = zeros(n,2);
r_sor = zeros(1,n);
r_oszl = zeros(1,n);


% r_sor, r_oszl kp szamolasa
for k = 1:n,
    kp(k,1) = real(A(k,k));
    kp(k,2) = imag(A(k,k));

    r_sor(k) = sum(abs(A(k,:))) - abs(A(k,k));
    r_oszl(k) = sum(abs(A(:,k))) - abs(A(k,k));
end


% rajzolas
x = 0:1/256:2*pi;
hold on;

for k = 1:n,
    % kozeppont zolddel
    plot(kp(k,1),kp(k,2),'og');

    % r_sor sugaru korok kekkel
    fill(r_sor(k)*cos(x)+kp(k,1),r_sor(k)*sin(x)+kp(k,2),'b');

    waitforbuttonpress;

    % r_oszl sugaru korok pirossal
    fill(r_oszl(k)*cos(x)+kp(k,1),r_oszl(k)*sin(x)+kp(k,2),'r');

    waitforbuttonpress;
end

hold off;

end

%!demo
%! ## 3x3 Szimmetrikus matrix
%! clf;
%! A = [4 -1 3; -1 2 2; 3 2 1];
%! [kp,r_sor,r_oszl] = gersgorin(A)

%!demo
%! ## 6x6 tridiag(1,2,1) matrix
%! clf;
%! A = diag(2*ones(1,6)) + diag(ones(1,5),-1) + diag(ones(1,5),+1);
%! [kp,r_sor,r_oszl] = gersgorin(A)

%!demo
%! ## 5x5 Vandermonde matrix
%! clf;
%! A = fliplr(vander(1:5));
%! [kp,r_sor,r_oszl] = gersgorin(A)

%!demo
%! ## 3x3 komplex elemu matrix
%! clf;
%! A = [1+i 1 1; 1 4+4i 1; 1 1 -3-5i];
%! [kp,r_sor,r_oszl] = gersgorin(A)
