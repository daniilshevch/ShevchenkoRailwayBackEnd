public class ClientApiService: ServiceHierarchyAttribute { }
public class AdminApiService: ServiceHierarchyAttribute { }
public class SystemApiService: ServiceHierarchyAttribute { }
public class ClientApiMethod: MethodHierarchyAttribute { }
public class AdminApiMethod: MethodHierarchyAttribute { }

public class DtoHierarchyAttribute: Attribute { }
public class ClientDto: DtoHierarchyAttribute { }
public class AdminDto: DtoHierarchyAttribute { }