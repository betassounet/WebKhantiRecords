

function LoginUser() {
    this.title = "";
    this.email="";
    this.password = "";
    this.droit = ""

    var _this = this;

    // Attention fonction appelé dans contexte different, d'ou perte du this..
    this.function1 = function () {
        _this.droit = "";
    }
}

//------------------------------------------------------------------------------------------------------------------------------------
// fonctions prototypes :
// A voir si judicieux pour objets qui ne sont instanciés qu'une seule fois..
//------------------------------------------------------------------------------------------------------------------------------------
LoginUser.prototype.Init = function (title, email) {
    this.title = title;
    this.email = email;
}


function LoginStatut() {
    this.loginUser = new LoginUser();

    this.currentNameArtiste = "";

    var _this = this;
}