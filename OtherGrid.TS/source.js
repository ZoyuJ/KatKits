function RemoveSpacesAndCRLF(Source) { return Source.replace(/(\r\n|\n|\r)\ */gm, ""); }

/**
 * 1. Create TableRender instance
 * 2. Call TableRender.PreRender : Create sub-render instances,call FetchData to alloc data
 *  2.1 Create sub-render instances : ColumnHeader, Pageination and TableData and Preset them (call Preset)
 *      Note: Preset is the method to alloc sub config to each sub-render, then grid can get init data which need to be calculated(something maybe not be writen by user in config definition)
 *  2.2 Fetch data 
 *      2.2.1 Call SetOptions from sub-render instances to update config
 *  2.3 call PreRender->Render from sub-renders to render data and ContextGenerating will be called in Render method
 *      2.3.1 Pager will update page number list
 *      2.3.2 TableData will update tbody
 *  2.4 at least call PostRender from instances step by step
 *      2.4.1 Pager will set page changed callback to chage page
 *          2.4.1.1 when page changed, call OnRequestPage then call DataSourceConditionsChanged to FetchData(2.2)
 *      2.4.2 ColumnHeader will set sorting config change callback
 *          2.4.2.1 when sorting config changed, call DataSourceConditionsChanged to FetchData(2.2)
 *          
 *          
 *  New TableRender -> TableRender.PreRender() = New SubRenders -> SubRenders.Preset() -> TableRender.FetchData() -> TableRender.Render -> SubRenders.PreRender().Render().PostRender() -> TableRender.PostRender() -(waiting input)-> DataSourceParamterChanged
 *                                                                                                       ↑(call)                                                                                                                                 │(event)
 *                                                                                                       └━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┙
 *                                                                                                                                                                                                                                  -> Change Row Mode -> RowRender.PreRender().Render().PostRender() -> CellValueChanged... -> Save Edited Row 
 *                                                                                                                                                                                                                                             ↑(Change to "Display")                                                                   │(event)
 *                                                                                                                                                                                                     Call TableRender.OnSaving  ━━━━━━━━━━━━━┷━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┙
 *                                                                                                                                                                                                                                  
 *                                                                                                                                                                                                                                  
 *  
 *  
 *  
 */


/**
const Cx = {
    TemplateSelector: { Display: "template#finaldeliveryproject-table", Edit: "" },
    ContextGenerating: null,
    OnPreRender: null,
    OnPostRender: null,
    //OnSavingItem: (Table:TableRender,Render:TableRender,NewData:any,OldData:any)=>{Data:any,Anythingelse:any}|null, //if has 'create' or 'edit' feature, this property cant be null, how to save created/modified item, have to return the response data
    //OnSavedItem: (Table:TableRender,Render:TableRender,Response:any)=>>void,  // anthing need to do if item saved?
    //OnDiscardingItem: (Render:TableRender,NewData:any,OldData:any)=>void|null, //if has 'create' or 'edit' feature, this property cant be null, how to cancel created/modified item
    Columns: {
        TemplateSelector: "template#finaldeliveryproject-header", //<- selector string for jquery
        Headers: [
            {
                Header: "Id", //<- text that will show in column header, but depends ur template, u can specify anything u wanna show in header
                Field: "Id", //<- field name of item, but we dont care what u fill here, oh we will use this field to specify the sorting direction changed, pass it to the FetchData that u defined, anyway we dont care what u filled here
                Sorting: "Disable", //<- can be sorting? Disable: cant Enable:can but no sorting DESC:DESC ASC:ASC
            },
            {
                Header: "This is Val",
                Field: "Val",
                Sorting: "Disable",
            }
        ],
        ContextGenerating: null,
        OnPreRender: null,
        OnPostRender: null,
        RenderOptions: { //<- check out Markup-js doc
            pipes: {},
            options: {}
        }
    },
    Row: { //<- define cfg of tr in tbody
        TemplateSelector: { 
            Display: "template#finaldeliveryproject-displayrow", //<- selector string for jquery
            Edit: "" //<- selector string for jquery
        },
        Cells: [ //<- define cfg of td in tr , and use same field name order as Columns cfg, but who cares, we didnt do any check on the config var, if u found some columns missed or overclapping u will double check the cfg right?! Columns cfg and this cfg just config column header and data one by one
            {
                Field: "Id", //<- Field name of resposed data item, can be unique or not, if wanna create more column with data but different config
                Display: {
                    TemplateSelector: "template#finaldeliveryproject-displaycell", //<- selector string for jquery
                    ContextGenerating:null, //<- (CellRender)=>object|object|null|undefine| //generate context to render ur template, param CellRender is type of TableCellRender
                    OnPreRender: null, //<- (CellRender)=>any|never
                    OnPostRender: null // <- (CellRender)=>any|never
                },
                Edit:{
                    EditControlType:"text", // <- string, anytype just what u like to define it
                    //...,
                    OnPostRender: null // <- (CellRender)=>any|never  //if has edit feature, should register value changed event here, just call row element.data("other-grid-row").OnEditing(<Field>,<Val>);
                }
            },
            {
                Field: "Val",
                Display: {
                    TemplateSelector: "template#finaldeliveryproject-displaycell",
                    ContextGenerating: null,
                    OnPreRender: null,
                    OnPostRender: null
                }
            }
        ],
        ContextGenerating: null, 
        OnPreRender: (Render) => {
            Render.Template = Render.Template.replace("<td>{{RenderedCells}}{{.}}{{/RenderedCells}}</td>", "{{RenderedCells}}{{.}}{{/RenderedCells}}");
        },
        RenderOptions: {
            pipes: {}
        }
    },
    Data: [], //<- keep empty
    OnBeforeFetchData: (Render) => void, //<- render start to fetch data (befor call fetchdata callback in config)
    OnAfterFetchDataSuccess: (Render, Response) => void, //<- will be called in ajax success, items and paginationinfo will be set to render automatically, and u can handle something else in response data here 
    OnAfterFetchDataFail: (Render, Response) => void,  //<- will be called in ajax fail or 'Items','PaginationInfo' member not in response data
    FetchData: (PagerContext, ColumnHeaderContext) => {}, //<- return $.ajax function's parameter  //param PagerContext come from Pageination.GetPageNavInfo; param ColumnHeaderContext come from ColumnHeaderRender.GetHeaderSortingInfo; matbe we will expose callbacks to let use process their own request body later?
    //OnDataFetched:null, // <- (Response)=>{Items:any[],PageinationInfo:any}|null
    Pageination: { //<- define cfg of pageination
        TemplateSelector: "template#finaldeliveryproject-pager",
        PerviousItemCountPerPage: null, //<- level null,0 or something else, this number will be overwrite by items count per page canged callback , no this feature right now btw.
        PerviousPageNumber: null, //<- level null,0 or something else, this number will be overwrite by page number clicked callback
        RequirePageNumber: 1, //<- which page we need? this number will pass to FetchData method at init step to fetch data
        TotalItemCount: 0,  //<- level null,0 or something else, this number will overwrite after FetchData everytime
        ItemCountPerPage: 20, //<- how many items per page we need? this number will pass to FetchData method at init step to fetch data
        ContextGenerating: null,
        OnRequestPage: null, 
        OnPageResponsed: null,
        OnPreRender: null,
        OnPostRender: null,
        RenderOptions: {
            pipes: {}
        }
    },
    RenderOptions: {
        pipes: { },
        options: {}
    }
}
 */


/**
 * render table element
 * @@param RootElement where is table
 * @@param Config
 */
function TableRender(RootElement, Config) {
    this.Config = Config;
    this.RootElement = RootElement;

    this.HeaderCollection = null;
    this.RowCollection = null;
    this.Pager = null;
    this.Template = null;
    this.Element = null;
    this.PreRender = () => {
        this.Template = GetTemplate(this.Config.TemplateSelector.Display);
        this.HeaderCollection = new ColumnHeaderRender(this).Preset();
        this.Pager = new PageinationRender(this).Preset();
        this.RowCollection = new TableDataRender(this).Preset();
        this.Config.OnPreRender && this.Config.OnPreRender(this);
        //this.FetchData();
        return this;
    };
    this.FetchData = () => {
        this.Config.OnBeforeFetchData(this);
        const Aja = this.Config.FetchData(this.Pager.GetPageNavInfo(), this.HeaderCollection.GetHeaderSortingInfo());
        __Render = this;
        Aja.success =function(R) {
            if (R.PageinationInfo && R.Items) {
                __Render .Pager.SetOptions(R.PageinationInfo);
                __Render.RowCollection.SetOptions(R.Items);
                __Render.Pager.OnPageResponsed();
                __Render.RowCollection.OnPageResponsed();
                __Render.Config.OnAfterFetchDataSuccess(__Render, R);
            }
            else {
                __Render.Config.OnAfterFetchDataFail(__Render , R);
            }
        };
        Aja.error = function(R) {
            __Render.Config.OnAfterFetchDataFail(__Render , R);
        }
        $.ajax(Aja);
    }
    this.ContextGenerating = () => {
        if (this.Config.ContextGenerating) {
            if (this.Config.ContextGenerating instanceof Function) return this.Config.ContextGenerating(this);
            else this.Config.ContextGenerating ?? { Context: this }
        }
        return { Context: this };
    }
    this.Render = () => {
        this.Element = $(Mark.up(this.Template, this.ContextGenerating(), this.Config.RenderOptions)).appendTo(this.RootElement);
        this.HeaderCollection.SetContainer(this.Element.find("thead").children("tr")).PreRender().Render().PostRender();
        this.RowCollection.SetContainer(this.Element.find("tbody")).PreRender().Render().PostRender();
        this.Pager.SetContainer(this.RootElement).PreRender().Render().PostRender();
        return this;
    };
    this.PostRender = () => {
        this.RootElement.data("othergrid", this);
        return this;
    };

    this.DataSourceConditionsChanged = () => {
        this.FetchData();
    }

    this.UpdateRow = (Data, Mode) => {

        const Row = this.RowCollection.Rows.find(E => E.Data.Id == Data.Id);
        if (Row) {
            const I = this.Config.Data.findIndex(E => E.Id == Data.Id);
            if (I != -1) {
                this.Config.Data[I] = Data;
            if (Mode) Row.DisplayStatus = Mode;
            Row.Data = this.Config.Data[I];
                Row.PreRender().Render().PostRender();
                return true;
            }

        }
        return false;
    }

    this.PreRender();
    this.Config.OnBeforeFetchData && this.Config.OnBeforeFetchData(this);
    const Aja = this.Config.FetchData(this.Pager.GetPageNavInfo(), this.HeaderCollection.GetHeaderSortingInfo());
    __Render = this;
    Aja.success = function (R) {
        if (R.PageinationInfo && R.Items) {
            __Render.Pager.SetOptions(R.PageinationInfo);
            __Render.RowCollection.SetOptions(R.Items);
            __Render.Render().PostRender();
            __Render.Config.OnAfterFetchDataSuccess(__Render, R);
        }
        else {
            __Render.Config.OnAfterFetchDataFail(__Render, R);
        }
    };
    Aja.error = function (R) {
        __Render.Config.OnAfterFetchDataFail(__Render, R);
    }
    $.ajax(Aja);
}

function GetTemplate(Selector) {
    return Selector instanceof Function ? Selector() : Selector.indexOf("<") != -1 ? Selector : $(Selector).html();
}

/**
 * the pager
 * @@param Config
 */
function PageinationRender(Grid) {
    this.Grid = Grid;
    this.Container = null;
    this.Config = null;
    this.Element = null;
    this.Template = null;
    this.Preset = () => {
        this.Config = this.Grid.Config.Pageination;
        return this;
    }
    this.SetOptions = (New) => {
        this.Config = this.Grid.Config.Pageination;
        if (this.Config.TotalItemCount !== New.TotalItemCount) {
            this.Config.TotalItemCount = New.TotalItemCount;
            this.ContextHasUpdated = true;
        }
        if ((!this.CurrentPageNumber) || this.CurrentPageNumber !== New.RequirePageNumber) {
            this.CurrentPageNumber = New.RequirePageNumber;
            this.Config.RequirePageNumber = null;
            this.ContextHasUpdated = true;
        }
    };
    this.SetContainer = (Container) => { this.Container = Container; return this; };
    this.OnPageResponsed = () => {
        this.PreRender().Render().PostRender();
    };
    this.PreRender = () => {
        this.Template = GetTemplate(this.Config.TemplateSelector);
        this.Config.OnPreRender && this.Config.OnPreRender(this);
        return this;
    };
    this.ContextGenerating = () => {
        if (this.Config.ContextGenerating) {
            if (this.Config.ContextGenerating instanceof Function) return this.Config.ContextGenerating(this);
            else this.Config.ContextGenerating ?? { Context: this }
        }
        return { Context: this };
    }
    this.Render = () => {
        if (this.Element) this.Element.remove();
        this.Element = $(Mark.up(this.Template, this.ContextGenerating(), this.Config.RenderOptions)).appendTo(this.Container);
        return this;
    }
    this.PostRender = () => {
        this.Element.find("a[data-othergrid-pageination-changeto]").click((e) => {
            const P = $(e.target).data("othergrid-pageination-changeto");
            this.Config.PerviousPageNumber = this.CurrentPageNumber;
            switch (P) {
                case "=1":
                    this.Config.RequirePageNumber = 1;
                    break;
                case "=-1":
                    this.Config.RequirePageNumber = this.TotalPageCount;
                    break;
                case "+1":
                    this.Config.RequirePageNumber = this.CurrentPageNumber + 1;
                    break;
                case "-1":
                    this.Config.RequirePageNumber = this.CurrentPageNumber - 1;
                    break;
                default:
                    this.Config.RequirePageNumber = parseInt(P);
                    break;
            }
            
            this.Config.OnRequestPage && this.Config.OnRequestPage(this);
            this.Grid.DataSourceConditionsChanged();
        });
        this.Config.OnPostRender && this.Config.OnPostRender(this);
    };

    this.CurrentPageNumber = null;
    this.ContextHasUpdated = null;
    this.IsLastPage = false;
    this.GetPageNavInfo = () => ({
        PerviousItemCountPerPage: this.Config.PerviousItemCountPerPage,
        PerviousPageNumber: this.Config.PerviousPageNumber,
        RequirePageNumber: (this.Config.RequirePageNumber==null || this.Config.RequirePageNumber == 0) ? this.CurrentPageNumber: this.Config.RequirePageNumber,
        ItemCountPerPage: this.Config.ItemCountPerPage
    });

}
/**
 * render column header
 * @@param Where first tr in thead
 * @@param Config
 */
function ColumnHeaderRender(Grid) {
    this.SORTING_CFG_NAME = ["Disbale", "Enable", "DESC", "ASC"]
    this.Grid = Grid;
    this.Config = null;
    this.Container = null;
    this.Element = null;
    this.Template = null;
    this.Preset = () => {
        this.Config = this.Grid.Config.Columns;
        return this;
    }
    this.SetContainer = (Container) => { this.Container = Container; return this; };
    this.PreRender = () => {
        this.Template = GetTemplate(this.Config.TemplateSelector);
        this.Config.PreRender && this.Config.PreRender(this);
        return this;
    };
    this.ContextGenerating = () => {
        if (this.Config.ContextGenerating) {
            if (this.Config.ContextGenerating instanceof Function) return this.Config.ContextGenerating(this);
            else this.Config.ContextGenerating ?? { Context: this }
        }
        return { Context: this };
    }
    this.Render = () => {
        if (this.Element) this.Element.remove();
        this.Element = $(Mark.up(this.Template, this.ContextGenerating(), this.Config.RenderOptions)).appendTo(this.Container);
        return this;
    }
    this.PostRender = () => {
        this.Element.find("[data-column-field]").click(e => {
            const F = $(e.target).data("column-field");
            const D = this.Config.Headers.find(E => E.Field === F);
            if (D !== "Disbale") {
                const K = this.SORTING_CFG_NAME.findIndex(E => E == D);
                K += 1;
                if (K > 3) K = 1;
                D.Sorting = this.SORTING_CFG_NAME[K];
                this.PreRender().Render().PostRender();
                this.Grid.DataSourceConditionsChanged();
            }
        });
        return this;
    };

    this.GetHeaderSortingInfo = () => {
        return this.Config.Headers.filter(E => E.Sorting > 1).map(E => ({ Field: E.Field, Sorting: E.Sorting }));
    }
}


/**
 * render tbody, loop context.data here
 * @@param Config
 * @@param Data
 */
function TableDataRender(Grid) {
    this.Grid = Grid;
    this.Config = null;
    this.Data = null;
    this.Container = null;
    this.Rows = [];

    this.Preset = () => {
        this.Config = this.Grid.Config.Row;
        this.Data = this.Grid.Config.Data;
        return this;
    }
    this.SetOptions = (New) => {
        this.Config = this.Grid.Config.Row;
        this.Grid.Config.Data = New;
        this.Data = this.Grid.Config.Data;
    }
    this.SetContainer = (Container) => { this.Container = Container; return this; };
    this.OnPageResponsed = () => {
        this.PreRender().Render().PostRender();
    };
    this.PreRender = () => {
        this.Rows = this.Data.map(Item => new TableRowRender(this.Grid,this.Config, Item));
        this.Config.PreRender && this.Config.PreRender(this);
        return this;
    };
    this.Render = () => {
        this.Container.empty();
        this.Rows.forEach(Item => {
            Item.SetContainer(this.Container).PreRender().Render().PostRender();
        });
        return this;
    };
    this.PostRender = () => {
        this.Config.PostRender && this.Config.PostRender(this);
        return this;
    };


}

//const ROW_STATES;
//(function (ROW_STATES) {
//    ROW_STATES[ROW_STATES["Normal"] = 0] = "Normal";
//    ROW_STATES[ROW_STATES["Created"] = 1] = "Created";
//    ROW_STATES[ROW_STATES["Deleted"] = 2] = "Deleted";
//})(ROW_STATES);
//const DISPLAY_STATES;
//(function (ROW_STATES) {
//    ROW_STATES[ROW_STATES["Normal"] = 0] = "Normal";
//    ROW_STATES[ROW_STATES["Created"] = 1] = "Edit";
//})(ROW_STATES);

/**
 * render one row and all nested cells (display only)
 * assign context and data for each cell, then render td htmltext then render tr tag with td htmltext
 * @@param Config
 * @@param Data
 */
function TableRowRender(Grid,Config, Data, DisplayStatus) {
    this.DisplayStatus = DisplayStatus ?? "Display";
    this.Grid = Grid;
    this.Config = Config;
    this.Data = Data;
    this.Container = null;
    this.Element = null;
    this.Cells = [];

    this.Template = null;
    this.SetContainer = (Container) => { this.Container = Container; return this; };
    this.PreRender = () => {
        this.Template = GetTemplate(this.Config.TemplateSelector[this.DisplayStatus]);
        this.Cells = this.Config.Cells.map(C => {
            C.RenderOptions = this.Config.RenderOptions;
            return new TableCellRender(C, this.Data, this, this.DisplayStatus);
        });
        this.Config.OnPreRender && this.Config.OnPreRender(this);
        return this;
    };
    this.ContextGenerating = () => {
        if (this.Config.ContextGenerating) {
            if (this.Config.ContextGenerating instanceof Function) return this.Config.ContextGenerating(this);
            else this.Config.ContextGenerating ?? { Context: this }
        }
        return { Context: this };
    }
    this.Render = () => {
        const NewEle = $(Mark.up(this.Template, this.ContextGenerating(), this.Config.RenderOptions));
        if (this.Element) {
            NewEle.insertAfter(this.Element);
            this.Element.remove();
            this.Element = NewEle;
        }
        else {
            this.Element = NewEle.appendTo(this.Container);
        }
        this.Cells.forEach(C => C.SetContainer(this.Element).PreRender().Render().PostRender());
        return this;
    };
    this.PostRender = () => {
        this.Element.data("other-grid-row", this);
        this.Config.OnPostRender && this.Config.OnPostRender(this);
        return this;
    };

    /**
     * change to config which u defined in Row.Cells[]
     * @param {any} NewMode
     */
    this.ChangeDisplayMode = (NewMode) => {
        if (this.DisplayStatus != NewMode) {
            this.DisplayStatus = NewMode;
            this.PreRender().Render().PostRender();
        }
    }

    //this.CopiedData = null;
    //this.OnEditing = (Field,Val) => {
    //    if (!this.CopiedData) this.CopiedData = JSON.parse(JSON.stringify(this.Data));
    //    this.CopiedData[Field] = Val;
    //}
    //this.OnSavingModification = () => {
    //    let OldData = null;
    //    if (this.CopiedData.Id && this.CopiedData.Id==0 ) {
    //        //How to proc new item?
    //    }
    //    else {
    //        const I = this.Grid.Config.Data.findIndex(E => E.Id == this.CopiedData.Id);
    //        if (I > -1) {
    //            OldData = this.Grid.Config.Data[I];
    //            const SavedData = this.Grid.OnSavingRow(this, this.CopiedData, OldData);
    //            this.Grid.Config.Data[I] = SavedData ;
    //            this.Data = this.Grid.Config.Data[I];
    //            this.Grid.OnSavedRow(this, SavedData);
    //        }
    //    }
        
    //}
    //this.OnDiscardingModification = () => {
    //    this.CopiedData = null;
    //    this.Grid.OnDiscardingRow(this);
    //}
}


/**
 * render td
 * @@param Config
 * @@param Data
 */
function TableCellRender(Config, Data, RowRender ,DisplayStatus = "Display") {
    this.DisplayStatus = DisplayStatus;
    this.RowRender = RowRender;
    this.Config = Config;
    this.Data = Data;
    this.Value = Data[Config.Field];
    this.Element = null;
    this.Template = null;
    this.SetContainer = (Container) => { this.Container = Container; return this; };
    this.PreRender = () => {
        this.Template = GetTemplate(this.Config[this.DisplayStatus].TemplateSelector);
        this.Config[this.DisplayStatus].OnPreRender && this.Config[this.DisplayStatus].OnPreRender(this);
        return this;
    };
    this.ContextGenerating = () => {
        if (this.Config[this.DisplayStatus].ContextGenerating) {
            if (this.Config[this.DisplayStatus].ContextGenerating instanceof Function) return this.Config[this.DisplayStatus].ContextGenerating(this);
            else this.Config[this.DisplayStatus].ContextGenerating ?? { Context: this }
        }
        return { Context: this };
    }
    this.Render = () => {
        if (this.Element) this.Element.remove();
        this.Element = $(Mark.up(this.Template, this.ContextGenerating(), this.Config.RenderOptions));
        this.Element.appendTo(this.Container);
        return this;
    };
    this.PostRender = () => {
        this.Config[this.DisplayStatus].OnPostRender && this.Config[this.DisplayStatus].OnPostRender(this);
        return this;
    }
}


window.OtherGrid =(JQueryHTMLElement, Config)=> new TableRender(JQueryHTMLElement, Config);