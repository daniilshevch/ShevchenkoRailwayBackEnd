public class ClientApiService: ServiceHierarchyAttribute { }
public class AdminApiService: ServiceHierarchyAttribute { }
public class ClientApiMethod: MethodHierarchyAttribute { }
public class AdminApiMethid: MethodHierarchyAttribute { }

public class DtoHierarchyAttribute: Attribute { }
public class ClientDto: DtoHierarchyAttribute { }
public class AdminDto: DtoHierarchyAttribute { }