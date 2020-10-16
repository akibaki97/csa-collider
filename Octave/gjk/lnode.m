classdef lnode < handle

  properties
    data;
    next;
  endproperties

  methods

    function this = lnode (a)
        if nargin == 0, this.data = [];
        else this.data = a;
        endif
        
        this.next = []; 
    endfunction

  endmethods

endclassdef