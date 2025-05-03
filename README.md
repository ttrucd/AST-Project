## Abstract Syntax Trees (AST) Project
# Thanh Dang

A single web application that parses MicroML code and draws Abstract Syntax Trees (AST).

# Tech Stack
Backend - F#, ASP.NET Core (Giraffe)
Frontend - HTML, CSS, JavaScript, D3.js
JSON - transferring the AST from backend to frontend

# Features
MicroML code parser with lexer and parser.
AST JSON output display
AST Tree visualization using D3.js
Custom notes explaining syntax rules. 

# Syntax Notes

- Every `let` expression must **end with `end`**.
- Programs must begin with `in` followed by an expression block.
- Nested expressions follow MicroML syntax.

Example:
let x = 5 in
  let y = 10 in
    x + y
  end
end

# Screenshots of the web app 
![image](https://github.com/user-attachments/assets/b2e171a1-e7c4-442a-a308-ddbdac0d023f)

![image](https://github.com/user-attachments/assets/5a7b68f0-317e-4191-8570-bf71221bb0d1)





