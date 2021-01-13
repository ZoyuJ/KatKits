
import { AsyncAction2 } from "./../node_modules/katkits.ts/lib/Event"
export default class FileUpLoader{

  constructor(Files:FileItem[]) {
    
  }


  public Push() {

  }

  public async Upload(): Promise<void> {

  }
  /*
      Slice(blob),PartSize,TotalSize,PartIndex,PartCount
   */
}
/**
 * 
 * */
export interface FileItem extends File {
  InputName?: string | null;
  
}